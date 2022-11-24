using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

/// <summary>
/// PeInfo 的摘要说明
/// </summary>
public class PeInfo
{
    /// <summary>
    /// 全部文件数据
    /// </summary>
    private byte[] PEFileByte;

    private bool _OpenFile = false;

    /// <summary>
    /// 获取是否正常打开文件 
    /// </summary>
    public bool OpenFile
    {
        get { return _OpenFile; }
    }

    /// <summary>
    /// 文件读取的位置
    /// </summary>
    private long PEFileIndex = 0;

    private bool isPE32 = true;
    public DosHeader _DosHeader;
    public DosStub _DosStub;
    public PEHeader _PEHeader;
    public OptionalHeader _OptionalHeader;
    public OptionalDirAttrib _OptionalDirAttrib;
    public SectionTable _SectionTable;
    public ExportDirectory _ExportDirectory;
    public ImportDirectory _ImportDirectory;
    public ResourceDirectory _ResourceDirectory;

    public PeInfo(string FileName)
    {
        _OpenFile = false;

        System.IO.FileStream File = new FileStream(FileName, System.IO.FileMode.Open);
        PEFileByte = new byte[File.Length];
        File.Read(PEFileByte, 0, PEFileByte.Length);
        File.Close();
        LoadFile();

        _OpenFile = true;
    }

    #region 读表方法

    /// <summary>
    /// 开始读取
    /// </summary>
    private void LoadFile()
    {
        LoadDosHeader(); //取DOS
        LoadDosStub();
        LoadPEHeader();
        LoadOptionalHeader();
        LoadOptionalDirAttrib();
        LoadSectionTable(); //获取节表

        LoadExportDirectory(); //获取输出表
        LoadImportDirectory(); //获取输入表
        LoadResourceDirectory();
    }

    void LoadDosHeader()
    {
        _DosHeader = new DosHeader();
        LoadBytesByType(_DosHeader);
    }

    private void LoadBytesByType(object obj, int startI = 0)
    {
        Type clsType = obj.GetType();
        var fields = clsType.GetFields();
        for (int i = startI; i < fields.Length; i++)
        {
            var field = fields[i];
            var attributes = field.GetCustomAttributes(false);
            if (attributes.Length == 0)
                continue;
            HeadAttribute dmAttr = (HeadAttribute)attributes[0];
            var size = isPE32 ? dmAttr.x32Size : dmAttr.x64Size;
            if (size == 0)
                continue;
            var data = Loadbyte(size);
            field.SetValue(obj, data);
        }
    }

    /// <summary>
    /// 获得DOS SUB字段 
    /// </summary>
    private void LoadDosStub()
    {
        long Size = GetLong(_DosHeader.e_lfanew) - PEFileIndex;
        _DosStub = new DosStub(Size);
        Loadbyte(ref _DosStub.DosStubData);
    }

    /// <summary>
    /// 获得PE的文件头
    /// </summary>
    /// <param name="Fileindex"></param>
    /// <returns></returns>
    private void LoadPEHeader()
    {
        _PEHeader = new PEHeader();
        LoadBytesByType(_PEHeader);
    }

    /// <summary>
    /// 获得OPTIONAL PE扩展属性
    /// </summary>
    /// <param name="Fileindex"></param>
    /// <returns></returns>
    private void LoadOptionalHeader()
    {
        _OptionalHeader = new OptionalHeader();
        Loadbyte(ref _OptionalHeader.Magic);
        isPE32 = GetInt2(_OptionalHeader.Magic) == 0x10b;
        LoadBytesByType(_OptionalHeader, 1);
    }

    /// <summary>
    /// 获取目录表
    /// </summary>
    /// <param name="Fileindex"></param>
    /// <returns></returns>
    private void LoadOptionalDirAttrib()
    {
        _OptionalDirAttrib = new OptionalDirAttrib();
        int DirCount = GetInt2(_OptionalHeader.NumberOfRvaAndSizes);

        for (int i = 0; i != DirCount; i++)
        {
            OptionalDirAttrib.DirAttrib DirectAttrib = new OptionalDirAttrib.DirAttrib();
            LoadBytesByType(DirectAttrib);

            _OptionalDirAttrib.DirByte.Add(DirectAttrib);
        }
    }

    /// <summary>
    /// 获取节表
    /// </summary>
    private void LoadSectionTable()
    {
        _SectionTable = new SectionTable();
        long Count = GetLong(_PEHeader.NumberOfSections);
        for (long i = 0; i != Count; i++)
        {
            SectionTable.SectionData Section = new SectionTable.SectionData();
            LoadBytesByType(Section);
            _SectionTable.Section.Add(Section);
        }
    }

    /// <summary>
    /// 读取输出表
    /// </summary>
    private void LoadExportDirectory()
    {
        if (_OptionalDirAttrib.DirByte.Count == 0) return;

        OptionalDirAttrib.DirAttrib ExporRVA = (OptionalDirAttrib.DirAttrib)_OptionalDirAttrib.DirByte[0];
        if (GetLong(ExporRVA.DirRva) == 0) return;

        //获取的位置
        long ExporAddress = GetLong(ExporRVA.DirRva);
        _ExportDirectory = new ExportDirectory();
        PEFileIndex = Rva2Fov(ExporAddress);
        LoadBytesByType(_ExportDirectory);

        PEFileIndex = Rva2Fov(_ExportDirectory.AddressOfFunctions);
        long Numb = GetLong(_ExportDirectory.NumberOfFunctions);
        for (long z = 0; z != Numb; z++)
        {
            byte[] Data = new byte[4];
            Loadbyte(ref Data);
            _ExportDirectory.AddressOfFunctionsList.Add(Data);
        }

        PEFileIndex = Rva2Fov(_ExportDirectory.AddressOfNames);
        Numb = GetLong(_ExportDirectory.NumberOfNames);

        for (long z = 0; z != Numb; z++)
        {
            byte[] Data = new byte[4];
            Loadbyte(ref Data);
            _ExportDirectory.AddressOfNamesList.Add(Data);
        }

        PEFileIndex = Rva2Fov(_ExportDirectory.AddressOfNameOrdinals);
        for (long z = 0; z != Numb; z++)
        {
            byte[] Data = new byte[2];
            Loadbyte(ref Data);
            _ExportDirectory.AddressOfNameOrdinalsList.Add(Data);
        }

        PEFileIndex = Rva2Fov(_ExportDirectory.Name);
        _ExportDirectory.DllName = LoadAsciiName();

        foreach (var nameBytes in  _ExportDirectory.AddressOfNamesList)
        {
            PEFileIndex = Rva2Fov(nameBytes);
            _ExportDirectory.NameList.Add(LoadAsciiName());
        }
        
        // while (true)
        // {
        //     var name = LoadAsciiName();
        //     if (name == null)
        //         break;
        //     _ExportDirectory.NameList.Add(name);
        // }
    }


    /// <summary>
    /// 读取输入表
    /// 
    private void LoadImportDirectory()
    {
        if (_OptionalDirAttrib.DirByte.Count < 1) return;

        OptionalDirAttrib.DirAttrib ImporRVA = (OptionalDirAttrib.DirAttrib)_OptionalDirAttrib.DirByte[1];

        //获取的位置
        long ImporAddress = GetLong(ImporRVA.DirRva);
        if (ImporAddress == 0) return;
        //获取大小
        long ImporSize = GetLong(ImporRVA.DirSize);

        _ImportDirectory = new ImportDirectory();

        long SizeRva = 0;
        long PointerRva = 0;

        long StarRva = 0;
        long EndRva = 0;


        for (int i = 0; i != _SectionTable.Section.Count; i++) //循环节表
        {
            SectionTable.SectionData Sect = (SectionTable.SectionData)_SectionTable.Section[i];

            StarRva = GetLong(Sect.VirtualAddress);
            EndRva = GetLong(Sect.SizeOfRawData);

            if (ImporAddress >= StarRva && ImporAddress < StarRva + EndRva)
            {
                SizeRva = GetLong(Sect.VirtualAddress);
                PointerRva = GetLong(Sect.PointerToRawData);
                PEFileIndex = ImporAddress - SizeRva + PointerRva;

                _ImportDirectory.FileStarIndex = PEFileIndex;
                _ImportDirectory.FileEndIndex = PEFileIndex + ImporSize;

                break;
            }
        }

        if (SizeRva == 0 && PointerRva == 0) return;


        while (true)
        {
            ImportDirectory.ImportDate Import = new PeInfo.ImportDirectory.ImportDate();
            LoadBytesByType(Import);

            if (GetLong(Import.Name) == 0) break;

            _ImportDirectory.ImportList.Add(Import); //添加
        }


        foreach (var Import in _ImportDirectory.ImportList)
        {
            PEFileIndex = Rva2Fov(Import.Name);
            Import.DLLName = LoadAsciiName();
            PEFileIndex = Rva2Fov(Import.OriginalFirstThunk);
            while (true)
            {
                ImportDirectory.ImportDate.FunctionList functionData =
                    new ImportDirectory.ImportDate.FunctionList();
                LoadBytesByType(functionData);
                if (GetLong(functionData.OriginalFirst) == 0) break;
                Import.DllFunctionList.Add(functionData);
            }

            foreach (var functionData in Import.DllFunctionList)
            {
                PEFileIndex = Rva2Fov(functionData.OriginalFirst);
                //如果设置了此位，则按序号导入。 否则，按名称导入
                if (PEFileIndex >> 31 == 0)
                {
                    Loadbyte(ref functionData.FunctionHead);
                    functionData.FunctionName = LoadAsciiName();
                }
                else
                {
                    functionData.FunctionName = new byte[1];
                    Loadbyte(ref functionData.FunctionHead);
                }
            }
        }
    }

    /// <summary>
    /// 读取资源表
    /// 
    private void LoadResourceDirectory()
    {
        #region 初始化

        if (_OptionalDirAttrib.DirByte.Count < 3) return;
        OptionalDirAttrib.DirAttrib ImporRVA = (OptionalDirAttrib.DirAttrib)_OptionalDirAttrib.DirByte[2];

        long ImporAddress = GetLong(ImporRVA.DirRva); //获取的位置
        if (ImporAddress == 0) return;
        long ImporSize = GetLong(ImporRVA.DirSize); //获取大小

        _ResourceDirectory = new ResourceDirectory();

        long SizeRva = 0;
        long PointerRva = 0;

        long StarRva = 0;
        long EndRva = 0;
        long PEIndex = 0;

        #endregion

        #region 获取位置

        for (int i = 0; i != _SectionTable.Section.Count; i++) //循环节表
        {
            SectionTable.SectionData Sect = (SectionTable.SectionData)_SectionTable.Section[i];

            StarRva = GetLong(Sect.VirtualAddress);
            EndRva = GetLong(Sect.SizeOfRawData);

            if (ImporAddress >= StarRva && ImporAddress < StarRva + EndRva)
            {
                SizeRva = GetLong(Sect.VirtualAddress);
                PointerRva = GetLong(Sect.PointerToRawData);
                PEFileIndex = ImporAddress - SizeRva + PointerRva;
                PEIndex = PEFileIndex;
                break;
            }
        }

        if (SizeRva == 0 && PointerRva == 0) return;

        #endregion

        AddResourceNode(_ResourceDirectory, PEIndex, 0, StarRva);
    }

    private void AddResourceNode(ResourceDirectory Node, long PEIndex, long RVA, long ResourSectRva)
    {
        PEFileIndex = PEIndex + RVA; //设置位置
        Loadbyte(ref Node.Characteristics);
        Loadbyte(ref Node.TimeDateStamp);
        Loadbyte(ref Node.MajorVersion);
        Loadbyte(ref Node.MinorVersion);
        Loadbyte(ref Node.NumberOfNamedEntries);
        Loadbyte(ref Node.NumberOfIdEntries);

        long NameRVA = GetLong(Node.NumberOfNamedEntries);
        for (int i = 0; i != NameRVA; i++)
        {
            ResourceDirectory.DirectoryEntry Entry = new ResourceDirectory.DirectoryEntry();
            Loadbyte(ref Entry.Name);
            Loadbyte(ref Entry.Id);

            byte[] Temp = new byte[2];
            Temp[0] = Entry.Name[0];
            Temp[1] = Entry.Name[1];

            long NameIndex = GetLong(Temp) + PEIndex;
            Temp[0] = PEFileByte[NameIndex + 0];
            Temp[1] = PEFileByte[NameIndex + 1];

            long NameCount = GetLong(Temp);
            Node.Name = new byte[NameCount * 2];
            for (int z = 0; z != Node.Name.Length; z++)
            {
                Node.Name[z] = PEFileByte[NameIndex + 2 + z];
            }

            Temp[0] = Entry.Id[2];
            Temp[1] = Entry.Id[3];

            long OldIndex = PEFileIndex;

            if (GetLong(Temp) == 0)
            {
                Temp[0] = Entry.Id[0];
                Temp[1] = Entry.Id[1];

                PEFileIndex = GetLong(Temp) + PEIndex;

                ResourceDirectory.DirectoryEntry.DataEntry DataRVA = new ResourceDirectory.DirectoryEntry.DataEntry();

                Loadbyte(ref DataRVA.ResourRVA);
                Loadbyte(ref DataRVA.ResourSize);
                Loadbyte(ref DataRVA.ResourTest);
                Loadbyte(ref DataRVA.ResourWen);

                PEFileIndex = OldIndex;
                Entry.DataEntryList.Add(DataRVA);
            }
            else
            {
                Temp[0] = Entry.Id[0];
                Temp[1] = Entry.Id[1];

                ResourceDirectory Resource = new ResourceDirectory();
                Entry.NodeDirectoryList.Add(Resource);
                AddResourceNode(Resource, PEIndex, GetLong(Temp), ResourSectRva);
            }

            PEFileIndex = OldIndex;

            Node.EntryList.Add(Entry);
        }

        long Count = GetLong(Node.NumberOfIdEntries);
        for (int i = 0; i != Count; i++)
        {
            ResourceDirectory.DirectoryEntry Entry = new ResourceDirectory.DirectoryEntry();
            Loadbyte(ref Entry.Name);
            Loadbyte(ref Entry.Id);

            byte[] Temp = new byte[2];
            Temp[0] = Entry.Id[2];
            Temp[1] = Entry.Id[3];

            long OldIndex = PEFileIndex;

            if (GetLong(Temp) == 0)
            {
                Temp[0] = Entry.Id[0];
                Temp[1] = Entry.Id[1];

                PEFileIndex = GetLong(Temp) + PEIndex;

                ResourceDirectory.DirectoryEntry.DataEntry DataRVA = new ResourceDirectory.DirectoryEntry.DataEntry();

                Loadbyte(ref DataRVA.ResourRVA);
                Loadbyte(ref DataRVA.ResourSize);
                Loadbyte(ref DataRVA.ResourTest);
                Loadbyte(ref DataRVA.ResourWen);

                PEFileIndex = OldIndex;
                Entry.DataEntryList.Add(DataRVA);
            }
            else
            {
                Temp[0] = Entry.Id[0];
                Temp[1] = Entry.Id[1];

                ResourceDirectory Resource = new ResourceDirectory();
                Entry.NodeDirectoryList.Add(Resource);
                AddResourceNode(Resource, PEIndex, GetLong(Temp), ResourSectRva);
            }

            PEFileIndex = OldIndex;

            Node.EntryList.Add(Entry);
        }
    }

    #endregion

    #region 类

    /// <summary>
    /// DOS文件都MS开始
    /// 
    public class DosHeader
    {
        [HeadAttribute("魔术数字", 2)] public byte[] e_magic;
        [HeadAttribute("文件最后页的字节数", 2)] public byte[] e_cblp;
        [HeadAttribute("文件页数", 2)] public byte[] e_cp;
        [HeadAttribute("重新定位", 2)] public byte[] e_crlc;
        [HeadAttribute("段落标题大小", 2)] public byte[] e_cparhdr;
        [HeadAttribute("所需的最小附加段", 2)] public byte[] e_minalloc;
        [HeadAttribute("所需的最大附加段", 2)] public byte[] e_maxalloc;
        [HeadAttribute("初始的SS值", 2)] public byte[] e_ss;
        [HeadAttribute("初始的SP值", 2)] public byte[] e_sp;
        [HeadAttribute("校验和", 2)] public byte[] e_csum;
        [HeadAttribute("初始的IP值", 2)] public byte[] e_ip;
        [HeadAttribute("初始的CS值", 2)] public byte[] e_cs;
        [HeadAttribute("重定位表的文件地址", 2)] public byte[] e_lfarlc;
        [HeadAttribute("套印号码", 2)] public byte[] e_ovno;
        [HeadAttribute("保留字", 8)] public byte[] e_res;
        [HeadAttribute("OEM标识符", 2)] public byte[] e_oemid;
        [HeadAttribute("OEM信息", 2)] public byte[] e_oeminfo;
        [HeadAttribute("保留字", 20)] public byte[] e_res2;
        [HeadAttribute("NT头开始的偏移量", 4)] public byte[] e_lfanew;
    }

    /// <summary>
    /// DOS程序 提示
    /// </summary>
    public class DosStub
    {
        public byte[] DosStubData;

        public DosStub(long Size)
        {
            DosStubData = new byte[Size];
        }
    }

    /// <summary>
    /// PE文件头
    /// </summary>
    public class PEHeader
    {
        [HeadAttribute("PE文件标记", 4)] public byte[] Header;

        [HeadAttribute("运行平台", 2, 2, "EnMachine")]
        public byte[] Machine;

        [HeadAttribute("节数", 2)] public byte[] NumberOfSections;

        [HeadAttribute("文件创建时间", 4, 4, "Time")]
        public byte[] TimeDateStamp;

        [HeadAttribute("COFF符号表的文件偏移量", 4)] public byte[] PointerToSymbolTable;
        [HeadAttribute("符号表中的条目数", 4)] public byte[] NumberOfSymbols;
        [HeadAttribute("可选标头的大小", 2)] public byte[] SizeOfOptionalHeader;

        [HeadAttribute("文件属性的标志", 2, 2, "EnImageCharacteristic")]
        public byte[] Characteristics;
    }

    /// <summary>
    /// Optinal
    /// </summary>
    public class OptionalHeader
    {
        [HeadAttribute("Magic数字", 2, 2, "EnMagic")]
        public byte[] Magic = new byte[2];

        [HeadAttribute("链接器主版本号", 1)] public byte[] MajorLinkerVersion;
        [HeadAttribute("链接器次版本号", 1)] public byte[] MinorLinkerVersion;
        [HeadAttribute("代码(文本)节的大小", 4)] public byte[] SizeOfCode;
        [HeadAttribute("数据节的大小", 4)] public byte[] SizeOfInitData;
        [HeadAttribute("未初始化数据大小", 4)] public byte[] SizeOfUninitData;
        [HeadAttribute("程序入口RVA", 4)] public byte[] AddressOfEntryPoint;
        [HeadAttribute("代码基址RVA", 4)] public byte[] BaseOfCode;
        [HeadAttribute("数据基址RVA", 4, 0)] public byte[] BaseOfData;
        [HeadAttribute("程序装载地址", 4, 8)] public byte[] ImageBase;
        [HeadAttribute("内存中区块对齐大小", 4)] public byte[] SectionAlignment;
        [HeadAttribute("文件中区块对齐大小", 4)] public byte[] FileAlignment;
        [HeadAttribute("操作系统主版本号", 2)] public byte[] MajorOperatingSysV;
        [HeadAttribute("操作系统次版本号", 2)] public byte[] MinorOperatingSysV;
        [HeadAttribute("映像的主版本号", 2)] public byte[] MajorImageVersion;
        [HeadAttribute("映像的次要版本号", 2)] public byte[] MinorImageVersion;
        [HeadAttribute("子系统的主版本号", 2)] public byte[] MajorSubsysV;
        [HeadAttribute("子系统的次版本号", 2)] public byte[] MinorSubsysV;
        [HeadAttribute("保留必须为零", 4)] public byte[] Win32VersionValue;
        [HeadAttribute("映象文件装入内存大小", 4)] public byte[] SizeOfImage;

        [HeadAttribute("MS-DOS存根PE标头和节标头的组合大小", 4)]
        public byte[] SizeOfHeards;

        [HeadAttribute("图像文件校验和", 4)] public byte[] CheckSum;

        [HeadAttribute("子系统", 2, 2, "EnSubsystem")]
        public byte[] Subsystem = new byte[2];

        [HeadAttribute("DLL特征", 2, 2, "EnDLLCharacteristic")]
        public byte[] DllCharacteristics = new byte[2];

        [HeadAttribute("保留堆栈的大小", 4, 8)] public byte[] SizeOfStackReserve;
        [HeadAttribute("提交堆栈的大小", 4, 8)] public byte[] SizeOfStackCommit;
        [HeadAttribute("保留本地堆空间大小", 4, 8)] public byte[] SizeOfHeapReserve;
        [HeadAttribute("要提交本地堆空间大小", 4, 8)] public byte[] SizeOfHeapCommit;
        [HeadAttribute("保留必须为零", 4)] public byte[] LoaderFlags;
        [HeadAttribute("数据目录条目数", 4)] public byte[] NumberOfRvaAndSizes;
    }

    /// <summary>
    /// 目录结构
    /// </summary>
    public class OptionalDirAttrib
    {
        public ArrayList DirByte = new ArrayList();

        public class DirAttrib
        {
            [HeadAttribute("内存中的地址", 4)] public byte[] DirRva;
            [HeadAttribute("内存中的大小", 4)] public byte[] DirSize;
        }
    }

    /// <summary>
    /// 节表
    /// </summary>
    public class SectionTable
    {
        public ArrayList Section = new ArrayList();

        public class SectionData
        {
            [HeadAttribute("名字", 8)] public byte[] SectName;
            [HeadAttribute("内存中的节的大小", 4)] public byte[] VirtualSize;
            [HeadAttribute("内充中的节起始地址", 4)] public byte[] VirtualAddress;
            [HeadAttribute("磁盘文件中节的大小", 4)] public byte[] SizeOfRawData;
            [HeadAttribute("磁盘文件中节的起始位置", 4)] public byte[] PointerToRawData;
            [HeadAttribute("文件指针指向分区重定位条目", 4)] public byte[] PointerToRelocations;
            [HeadAttribute("文件指针指向节的行号条目", 4)] public byte[] PointerToLinenumbers;
            [HeadAttribute("分区的重定位条目数", 2)] public byte[] NumberOfRelocations;
            [HeadAttribute("节的行号条目数", 2)] public byte[] NumberOfLinenumbers;
            [HeadAttribute("描述节特征的标志", 4)] public byte[] Characteristics;
        }
    }

    /// <summary>
    /// 输出表
    /// </summary>
    public class ExportDirectory
    {
        [HeadAttribute("导出标志(保留为0)", 4)] public byte[] Characteristics;
        [HeadAttribute("产生的时间", 4)] public byte[] TimeDateStamp;
        [HeadAttribute("主版本号", 2)] public byte[] MajorVersion;
        [HeadAttribute("次版本号", 2)] public byte[] MinorVersion;

        [HeadAttribute("名称RVA", 4)] public byte[] Name;
        [HeadAttribute("名称", 0, 0, "String")] public byte[] DllName;
        [HeadAttribute("序号基数", 4)] public byte[] Base;
        [HeadAttribute("地址表条目", 4)] public byte[] NumberOfFunctions;
        [HeadAttribute("名称指针数", 4)] public byte[] NumberOfNames;
        [HeadAttribute("导出地址表RVA", 4)] public byte[] AddressOfFunctions;
        [HeadAttribute("名称指针RVA", 4)] public byte[] AddressOfNames;
        [HeadAttribute("序号表RVA", 4)] public byte[] AddressOfNameOrdinals;

        public List<byte[]> AddressOfFunctionsList = new List<byte[]>();
        public List<byte[]> AddressOfNamesList = new List<byte[]>();
        public List<byte[]> AddressOfNameOrdinalsList = new List<byte[]>();
        public List<byte[]> NameList = new List<byte[]>();
    }

    /// <summary>
    /// 输入表
    /// </summary>
    public class ImportDirectory
    {
        public List<ImportDate> ImportList = new List<ImportDate>();

        public class ImportDate
        {
            [HeadAttribute("导入查阅表RVA", 4)] public byte[] OriginalFirstThunk;
            [HeadAttribute("指向时间标志", 4)] public byte[] TimeDateStamp;
            [HeadAttribute("指向链接索引", 4)] public byte[] ForwarderChain;
            [HeadAttribute("名称RVA", 4)] public byte[] Name;
            [HeadAttribute("导入地址表RVA", 4)] public byte[] FirstThunk;

            //DLL名称
            public byte[] DLLName;
            public List<FunctionList> DllFunctionList = new List<FunctionList>();

            public class FunctionList
            {
                [HeadAttribute("RVA", 4, 8)] public byte[] OriginalFirst;
                public byte[] FunctionHead = new byte[2];
                public byte[] FunctionName;
            }
        }

        public long FileStarIndex = 0;
        public long FileEndIndex = 0;
    }

    /// <summary>
    /// 资源表
    /// </summary>
    public class ResourceDirectory
    {
        public byte[] Characteristics = new byte[4];
        public byte[] TimeDateStamp = new byte[4];
        public byte[] MajorVersion = new byte[2];
        public byte[] MinorVersion = new byte[2];
        public byte[] NumberOfNamedEntries = new byte[2];
        public byte[] NumberOfIdEntries = new byte[2];

        public byte[] Name;

        public ArrayList EntryList = new ArrayList();

        public class DirectoryEntry
        {
            public byte[] Name = new byte[4];
            public byte[] Id = new byte[4];

            public ArrayList DataEntryList = new ArrayList();

            public ArrayList NodeDirectoryList = new ArrayList();

            public class DataEntry
            {
                public byte[] ResourRVA = new byte[4];
                public byte[] ResourSize = new byte[4];
                public byte[] ResourTest = new byte[4];
                public byte[] ResourWen = new byte[4];
            }
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 读数据 读byte[]的数量 会改边PEFileIndex的值
    /// </summary>
    /// <param name="Data"></param>
    private void Loadbyte(ref byte[] Data)
    {
        for (int i = 0; i != Data.Length; i++)
        {
            Data[i] = PEFileByte[PEFileIndex];
            PEFileIndex++;
        }
    }

    private byte[] Loadbyte(int size)
    {
        byte[] Data = new byte[size];
        for (int i = 0; i != size; i++)
        {
            Data[i] = PEFileByte[PEFileIndex];
            PEFileIndex++;
        }

        return Data;
    }

    private byte[] LoadAsciiName()
    {
        int ReadIndex = 0;
        while (true)
        {
            if (PEFileByte[PEFileIndex + ReadIndex] == 0)
            {
                if (ReadIndex == 0)
                    break;
                byte[] Date = new byte[ReadIndex];
                Loadbyte(ref Date);
                PEFileIndex += ReadIndex;
                return Date;
            }

            ReadIndex++;
        }

        return null;
    }

    /// <summary>
    /// 转换byte为字符串
    /// </summary>
    /// <param name="Data">byte[]</param>
    /// <returns>AA BB CC DD</returns>
    public static string GetString(byte[] Data)
    {
        if (Data == null)
            return "NULL";
        return BitConverter.ToString(Data.Reverse().ToArray()).Replace("-", "");
    }

    public static int GetInt2(byte[] Data)
    {
        if (Data.Length == 2)
            return BitConverter.ToInt16(Data, 0);
        return BitConverter.ToInt32(Data, 0);
    }

    /// <summary>
    /// 转换字符为显示数据
    /// </summary>
    /// <param name="Data">byte[]</param>
    /// <param name="Type">ASCII DEFAULT UNICODE BYTE</param>
    /// <returns></returns>
    public static string GetString(byte[] Data, string Type)
    {
        if (Type.Trim().ToUpper() == "ASCII") return System.Text.Encoding.ASCII.GetString(Data);
        if (Type.Trim().ToUpper() == "DEFAULT") return System.Text.Encoding.Default.GetString(Data);
        if (Type.Trim().ToUpper() == "UNICODE") return System.Text.Encoding.Unicode.GetString(Data);
        if (Type.Trim().ToUpper() == "BYTE")
        {
            string Temp = "";
            for (int i = Data.Length - 1; i != 0; i--)
            {
                Temp += Data[i].ToString("X02") + " ";
            }

            Temp += Data[0].ToString("X02");

            return Temp;
        }

        return GetInt(Data);
    }

    /// <summary>
    /// 转换BYTE为INT
    /// </summary>
    /// <param name="Data"></param>
    /// <returns></returns>
    public static string GetInt(byte[] Data)
    {
        return GetInt2(Data).ToString();
    }

    /// <summary>
    /// 转换数据为LONG
    /// </summary>
    /// <param name="Data"></param>
    /// <returns></returns>
    public static long GetLong(byte[] Data)
    {
        if (Data.Length == 2)
            return BitConverter.ToInt16(Data, 0);
        if (Data.Length == 4)
            return BitConverter.ToInt32(Data, 0);
        return BitConverter.ToInt64(Data, 0);
    }

    public SectionTable.SectionData GetSectionDataByRVA(long rva)
    {
        for (int i = 0; i != _SectionTable.Section.Count; i++) //循环节表
        {
            SectionTable.SectionData Sect = (SectionTable.SectionData)_SectionTable.Section[i];

            long StarRva = GetLong(Sect.VirtualAddress);
            long EndRva = GetLong(Sect.SizeOfRawData);

            if (rva >= StarRva && rva < StarRva + EndRva)
            {
                return Sect;
            }
        }

        return null;
    }

    public long Rva2Fov(byte[] data)
    {
        return Rva2Fov(GetLong(data));
    }

    public long Rva2Fov(long rva)
    {
        var section = GetSectionDataByRVA(rva);
        if (section == null)
            return 0;
        return rva - GetLong(section.VirtualAddress) + GetLong(section.PointerToRawData);
    }

    /// <summary>
    /// 添加一行信息
    /// 
    /// <param name="RefTable">表</param>
    /// <param name="Data">数据</param>
    /// <param name="Name">名称</param>
    /// <param name="Describe">说明</param>
    private void AddTableRow(DataTable RefTable, byte[] Data, string Name, string Describe)
    {
        RefTable.Rows.Add(new string[]
        {
            Name,
            Data.Length.ToString(),
            GetString(Data),
            GetLong(Data).ToString(),
            GetString(Data, "ASCII"),
            Describe
        });
    }

    private DataRow AddTableRow2(DataTable RefTable, string Name, byte[] Data, byte[] Size)
    {
        return RefTable.Rows.Add(new string[]
        {
            Name,
            GetString(Data),
            GetInt2(Data).ToString(),
            GetString(Size),
            GetInt2(Size).ToString()
        });
    }

    #endregion

    public Dictionary<string, List<ImportDirectory.ImportDate.FunctionList>> TableImportFunction()
    {
        Dictionary<string, List<ImportDirectory.ImportDate.FunctionList>> dictionary =
            new Dictionary<string, List<ImportDirectory.ImportDate.FunctionList>>();
        for (int i = 0; i != _ImportDirectory.ImportList.Count; i++)
        {
            ImportDirectory.ImportDate ImportByte = _ImportDirectory.ImportList[i];
            var list = dictionary[GetString(ImportByte.DLLName, "ASCII")] =
                new List<ImportDirectory.ImportDate.FunctionList>();
            for (int z = 0; z != ImportByte.DllFunctionList.Count; z++)
            {
                ImportDirectory.ImportDate.FunctionList Function = ImportByte.DllFunctionList[z];
                list.Add(Function);
            }
        }

        return dictionary;
    }

    public DataTable TableOptionalDirAttrib()
    {
        DataTable ReturnTable = new DataTable("");
        ReturnTable.Columns.Add("Name");
        ReturnTable.Columns.Add("RVA(16)");
        ReturnTable.Columns.Add("RVA(10)");
        ReturnTable.Columns.Add("Size(16)");
        ReturnTable.Columns.Add("Size(10)");

        Hashtable TableName = new Hashtable();

        TableName.Add("0", "导出表");
        TableName.Add("1", "导入表");
        TableName.Add("2", "资源表");
        TableName.Add("3", "异常表");
        TableName.Add("4", "安全表");
        TableName.Add("5", "基本重定位表");
        TableName.Add("6", "调试数据");
        TableName.Add("7", "版权数据");
        TableName.Add("8", "全局PTR");
        TableName.Add("9", "TLS表");
        TableName.Add("10", "加载配置表");
        TableName.Add("11", "绑定导入");
        TableName.Add("12", "IAT");
        TableName.Add("13", "延迟导入描述符");
        TableName.Add("14", "CLR 运行时标头");
        TableName.Add("15", "保留，必须为零");

        for (int i = 0; i != _OptionalDirAttrib.DirByte.Count; i++)
        {
            OptionalDirAttrib.DirAttrib MyDirByte = (OptionalDirAttrib.DirAttrib)_OptionalDirAttrib.DirByte[i];
            string Name = TableName[i.ToString()].ToString();

            AddTableRow2(ReturnTable, Name, MyDirByte.DirRva, MyDirByte.DirSize);
        }

        return ReturnTable;
    }

    public DataTable TableSectionData()
    {
        DataTable ReturnTable = new DataTable("");
        ReturnTable.Columns.Add("Name");
        ReturnTable.Columns.Add("Size");
        ReturnTable.Columns.Add("Value16");
        ReturnTable.Columns.Add("Value10");
        ReturnTable.Columns.Add("ASCII");
        ReturnTable.Columns.Add("Describe");

        for (int k = 0; k != _SectionTable.Section.Count; k++)
        {
            SectionTable.SectionData SectionDate = (SectionTable.SectionData)_SectionTable.Section[k];
            Type clsType = SectionDate.GetType();
            var fields = clsType.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var attribute = field.GetCustomAttributes(false);
                if (attribute.Length == 0)
                    continue;
                HeadAttribute dmAttr = (HeadAttribute)field.GetCustomAttributes(false)[0];
                AddTableRow(ReturnTable, field.GetValue(SectionDate) as byte[], field.Name, dmAttr.name);
            }
        }

        return ReturnTable;
    }
}

public class HeadAttribute : Attribute
{
    public readonly string name;
    public readonly string tpyeName;
    public readonly int x32Size;
    public readonly int x64Size;

    public HeadAttribute(string headName, int x32Size = 0, int x64Size = -1, string tpyeName = "")
    {
        this.name = headName;
        this.x32Size = x32Size;
        if (x64Size == -1)
            x64Size = x32Size;
        this.x64Size = x64Size;
        this.tpyeName = tpyeName;
    }
}