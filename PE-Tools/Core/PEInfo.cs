using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

/// <summary>
/// PeInfo 的摘要说明
/// </summary>
public unsafe class PeInfo
{
    /// <summary>
    /// 全部文件数据
    /// </summary>
    private BytesArray mByte;

    public BytesArray bytes
    {
        get => mByte;
    }

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
    private bool isPE32 = true;

    public DosHeader _DosHeader;
    public DosStub _DosStub;
    public PEHeader _PEHeader;
    public OptionalHeader _OptionalHeader;
    public OptionalDirAttrib _OptionalDirAttrib;
    public SectionTable _SectionTable;
    public ExportDirectory _ExportDirectory;
    public ImportDirectory _ImportDirectory;
    public CLRDirectory clrDirectory;

    public PeInfo(string FileName)
    {
        _OpenFile = false;
        System.IO.FileStream File = new FileStream(FileName, System.IO.FileMode.Open);
        var bytes = new byte[File.Length];
        File.Read(bytes, 0, bytes.Length);
        mByte = new BytesArray(bytes);
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
        LoadCLRDirectory(); //CLR表
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
            if (attributes[0] is ClassAttribute)
            {
                LoadBytesByType(field.GetValue(obj));
                continue;
            }

            if (attributes[0] is ByteAttribute)
            {
                field.SetValue(obj, mByte.ReadInt8());
                continue;
            }

            if (attributes[0] is ShortAttribute)
            {
                field.SetValue(obj, mByte.ReadInt16());
                continue;
            }

            if (attributes[0] is LongAttribute)
            {
                field.SetValue(obj, mByte.ReadInt64());
                continue;
            }

            if (attributes[0] is IntAttribute)
            {
                field.SetValue(obj, mByte.ReadInt32());
                continue;
            }

            if (attributes[0] is UintAttribute)
            {
                field.SetValue(obj, mByte.ReadUInt32());
                continue;
            }

            if (attributes[0] is StringAttribute)
            {
                StringAttribute strAttr = (StringAttribute)attributes[0];
                if (strAttr.name != "")
                {
                    var length = PETools.GetInt(GetFieldInfo(fields, strAttr.name).GetValue(obj) as byte[]);
                    field.SetValue(obj, mByte.ReadUTF8String(length));
                }
                else
                {
                    field.SetValue(obj, mByte.ReadUTF8String());
                }

                continue;
            }

            HeadAttribute dmAttr = (HeadAttribute)attributes[0];
            var size = isPE32 ? dmAttr.x32Size : dmAttr.x64Size;
            if (size == 0)
                continue;
            field.SetValue(obj, mByte.Loadbyte(size));
        }
    }

    FieldInfo GetFieldInfo(FieldInfo[] fields, string name)
    {
        foreach (var field in fields)
        {
            if (field.Name == name)
                return field;
        }

        return null;
    }

    /// <summary>
    /// 获得DOS SUB字段 
    /// </summary>
    private void LoadDosStub()
    {
        long Size = PETools.GetUint(_DosHeader.e_lfanew) - mByte.Position;
        _DosStub = new DosStub(Size);
        mByte.Loadbyte(ref _DosStub.DosStubData);
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
        mByte.Loadbyte(ref _OptionalHeader.Magic);
        isPE32 = PETools.GetInt(_OptionalHeader.Magic) == 0x10b;
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
        int DirCount = PETools.GetInt(_OptionalHeader.NumberOfRvaAndSizes);

        for (int i = 0; i != DirCount; i++)
        {
            DirAttrib DirectAttrib = new DirAttrib();
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
        long Count = PETools.GetUint(_PEHeader.NumberOfSections);
        for (long i = 0; i != Count; i++)
        {
            SectionData Section = new SectionData();
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

        DirAttrib ExporRVA = (DirAttrib)_OptionalDirAttrib.DirByte[0];
        if (PETools.GetUint(ExporRVA.DirRva) == 0) return;

        //获取的位置
        var ExporAddress = PETools.GetUint(ExporRVA.DirRva);
        _ExportDirectory = new ExportDirectory();
        mByte.Position = Rva2Fov(ExporAddress);
        LoadBytesByType(_ExportDirectory);

        mByte.Position = Rva2Fov(_ExportDirectory.AddressOfFunctions);
        var Numb = PETools.GetUint(_ExportDirectory.NumberOfFunctions);
        for (long z = 0; z != Numb; z++)
        {
            byte[] Data = new byte[4];
            mByte.Loadbyte(ref Data);
            _ExportDirectory.AddressOfFunctionsList.Add(Data);
        }

        mByte.Position = Rva2Fov(_ExportDirectory.AddressOfNames);
        Numb = PETools.GetUint(_ExportDirectory.NumberOfNames);

        for (long z = 0; z != Numb; z++)
        {
            byte[] Data = new byte[4];
            mByte.Loadbyte(ref Data);
            _ExportDirectory.AddressOfNamesList.Add(Data);
        }

        mByte.Position = Rva2Fov(_ExportDirectory.AddressOfNameOrdinals);
        for (long z = 0; z != Numb; z++)
        {
            byte[] Data = new byte[2];
            mByte.Loadbyte(ref Data);
            _ExportDirectory.AddressOfNameOrdinalsList.Add(Data);
        }

        mByte.Position = Rva2Fov(_ExportDirectory.Name);
        _ExportDirectory.DllName = mByte.ReadString();

        foreach (var nameBytes in _ExportDirectory.AddressOfNamesList)
        {
            mByte.Position = Rva2Fov(nameBytes);
            _ExportDirectory.NameList.Add(mByte.ReadString());
        }
    }


    /// <summary>
    /// 读取输入表
    /// 
    private void LoadImportDirectory()
    {
        if (_OptionalDirAttrib.DirByte.Count < 1) return;

        DirAttrib ImporRVA = (DirAttrib)_OptionalDirAttrib.DirByte[1];

        //获取的位置
        var ImporAddress = PETools.GetUint(ImporRVA.DirRva);
        if (ImporAddress == 0) return;

        _ImportDirectory = new ImportDirectory();
        mByte.Position = Rva2Fov(ImporAddress);

        while (true)
        {
            ImportDirectory.ImportDate Import = new ImportDirectory.ImportDate();
            LoadBytesByType(Import);

            if (PETools.GetUint(Import.Name) == 0) break;

            _ImportDirectory.ImportList.Add(Import); //添加
        }


        foreach (var Import in _ImportDirectory.ImportList)
        {
            mByte.Position = Rva2Fov(Import.Name);
            Import.DLLName = mByte.ReadString();
            mByte.Position = Rva2Fov(Import.OriginalFirstThunk);
            while (true)
            {
                ImportDirectory.ImportDate.FunctionList functionData =
                    new ImportDirectory.ImportDate.FunctionList();
                LoadBytesByType(functionData);
                if (PETools.GetLong(functionData.OriginalFirst) == 0) break;
                Import.DllFunctionList.Add(functionData);
            }

            foreach (var functionData in Import.DllFunctionList)
            {
                mByte.Position = Rva2Fov(functionData.OriginalFirst);
                //如果设置了此位，则按序号导入。 否则，按名称导入
                if (mByte.Position >> 31 == 0)
                {
                    mByte.Loadbyte(ref functionData.FunctionHead);
                    functionData.FunctionName = mByte.ReadString();
                }
                else
                {
                    functionData.FunctionName = new byte[1];
                    mByte.Loadbyte(ref functionData.FunctionHead);
                }
            }
        }
    }

    private void LoadCLRDirectory()
    {
        DirAttrib clrRVA = (DirAttrib)_OptionalDirAttrib.DirByte[14];
        var address = PETools.GetUint(clrRVA.DirRva); //获取的位置
        if (address == 0) return;
        mByte.Position = Rva2Fov(address);
        clrDirectory = new CLRDirectory();
        LoadBytesByType(clrDirectory);

        address = PETools.GetUint(clrDirectory.metaDir.DirRva); //获取的位置
        mByte.Position = Rva2Fov(address);
        var metadataHeader = clrDirectory.metadataHeader = new MetadataHeader();
        LoadBytesByType(metadataHeader);
        for (int i = 0; i < metadataHeader.streamLength; i++)
        {
            HeapStream heapStream = new HeapStream();
            LoadBytesByType(heapStream);
            mByte.Position = mByte.Position - 1 - (uint)heapStream.name.Length +
                             (uint)((heapStream.name.Length + 1 + 3) & ~3U);
            metadataHeader.streams.Add(heapStream);
        }

        foreach (var stream in metadataHeader.streams)
        {
            var startPos = Rva2Fov(address) + (uint)stream.offset;
            mByte.Position = startPos;
            stream.reader = new BytesArray(mByte, 0, (uint)stream.streamSize);
            switch (stream.name)
            {
                case "#~":
                case "#-":
                    TableStreamHeader tableStreamHeader = new TableStreamHeader();
                    LoadBytesByType(tableStreamHeader);
                    MDStreamFlags flags = (MDStreamFlags)tableStreamHeader.heapSizes;
                    bool HasBigStrings = (flags & MDStreamFlags.BigStrings) != 0;
                    bool HasBigGUID = (flags & MDStreamFlags.BigGUID) != 0;
                    bool HasBigBlob = (flags & MDStreamFlags.BigBlob) != 0;
                    TableStream tableStream = new TableStream();
                    var tableInfos = tableStream.CreateTables(tableStreamHeader.majorVersion,
                        tableStreamHeader.minorVersion, out int maxPresentTables);
                    var mdTables = new MDTable[tableInfos.Length];
                    var sizes = new uint[64];
                    var valid = tableStreamHeader.valid;

                    for (int i = 0; i < 64; valid >>= 1, i++)
                    {
                        uint rows = (valid & 1) == 0 ? 0 : mByte.ReadUInt32();
                        // Mono ignores the high byte
                        rows &= 0x00FFFFFF;
                        if (i >= maxPresentTables)
                            rows = 0;
                        sizes[i] = rows;
                        if (i < mdTables.Length)
                            mdTables[i] = new MDTable((Table)i, rows, tableInfos[i]);
                    }

                    uint offset = 0;
                    foreach (var table in mdTables)
                    {
                        table.UpdateSize(HasBigStrings, HasBigGUID, HasBigBlob, sizes, ref offset);
                    }

                    tableStream.reader = new BytesArray(mByte, 0, (uint)stream.streamSize);
                    tableStream.tablesPos = (uint)(mByte.Position - startPos);
                    tableStream.tables = mdTables;
                    MetadataHeader.tableStream = tableStream;
                    break;
                case "#Strings":
                    MetadataHeader.StringHeapStream = stream;
                    ReadStringList(stream);
                    break;
                case "#US":
                    MetadataHeader.UsHeapStream = stream;
                    ReadCompressedStringList(stream);
                    break;
                case "#Blob":
                    MetadataHeader.BlobHeapStream = stream;
                    ReadCompressedStringList(stream);
                    break;
                case "#GUID":
                    MetadataHeader.GuidHeapStream = stream;
                    ReadGuid(stream);
                    break;
                case "#Pdb":
                    MetadataHeader.PdbHeapStream = stream;
                    // ReadCompressedStringList(stream);
                    break;
                case "#JTD":
                    break;
            }
        }

        MetadataHeader.tableStream.InitializeTables();
        MetadataHeader.tableStream.Initialize();
    }

    void ReadGuid(HeapStream stream)
    {
        var offset = 0;
        while (true)
        {
            if (!stream.IsValid())
                break;
            HeapStreamString value = new HeapStreamString();
            value.value = stream.ReadGuid().ToByteArray();
            value.length = 16;
            value.offset = offset;
            stream.list.Add(value);
            offset += value.length;
        }
    }


    void ReadStringList(HeapStream stream)
    {
        var offset = 0;
        while (true)
        {
            var str = stream.TryReadBytesUntil();
            if (!stream.IsValid())
                break;
            HeapStreamString value = new HeapStreamString();
            value.length = str.Length;
            value.offset = offset;
            value.value = str;
            stream.list.Add(value);
            offset += value.length;
        }
    }

    void ReadCompressedStringList(HeapStream stream)
    {
        while (true)
        {
            if (!stream.IsValid())
                break;
            var pos = stream.reader.Position;
            byte[] str = stream.ReadUTF16String(pos);
            HeapStreamString value = new HeapStreamString();
            value.length = str.Length;
            value.offset = (int)pos;
            value.value = str;
            stream.list.Add(value);
        }
    }

    #endregion


    public SectionData GetSectionDataByRVA(long rva)
    {
        for (int i = 0; i != _SectionTable.Section.Count; i++) //循环节表
        {
            SectionData Sect = (SectionData)_SectionTable.Section[i];

            long StarRva = PETools.GetUint(Sect.VirtualAddress);
            long EndRva = PETools.GetUint(Sect.SizeOfRawData);

            if (rva >= StarRva && rva < StarRva + EndRva)
            {
                return Sect;
            }
        }

        return null;
    }

    public uint Rva2Fov(byte[] data)
    {
        return Rva2Fov(PETools.GetUint(data));
    }

    public uint Rva2Fov(uint rva)
    {
        var section = GetSectionDataByRVA(rva);
        if (section == null)
            return 0;
        return rva - PETools.GetUint(section.VirtualAddress) + PETools.GetUint(section.PointerToRawData);
    }
}