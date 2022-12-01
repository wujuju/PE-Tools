using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    public CLRDirectory clrDirectory;

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
                field.SetValue(obj, ReadInt8());
                continue;
            }

            if (attributes[0] is ShortAttribute)
            {
                field.SetValue(obj, ReadInt16());
                continue;
            }

            if (attributes[0] is LongAttribute)
            {
                field.SetValue(obj, ReadInt64());
                continue;
            }

            if (attributes[0] is IntAttribute)
            {
                field.SetValue(obj, ReadInt32());
                continue;
            }

            if (attributes[0] is StringAttribute)
            {
                StringAttribute strAttr = (StringAttribute)attributes[0];
                if (strAttr.name != "")
                {
                    var length = GetInt2(GetFieldInfo(fields, strAttr.name).GetValue(obj) as byte[]);
                    field.SetValue(obj, ReadString(length));
                }
                else
                {
                    field.SetValue(obj, ReadString());
                }

                continue;
            }

            HeadAttribute dmAttr = (HeadAttribute)attributes[0];
            var size = isPE32 ? dmAttr.x32Size : dmAttr.x64Size;
            if (size == 0)
                continue;
            var data = Loadbyte(size);
            field.SetValue(obj, data);
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
        long Count = GetLong(_PEHeader.NumberOfSections);
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

        foreach (var nameBytes in _ExportDirectory.AddressOfNamesList)
        {
            PEFileIndex = Rva2Fov(nameBytes);
            _ExportDirectory.NameList.Add(LoadAsciiName());
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
        long ImporAddress = GetLong(ImporRVA.DirRva);
        if (ImporAddress == 0) return;

        _ImportDirectory = new ImportDirectory();
        PEFileIndex = Rva2Fov(ImporAddress);

        while (true)
        {
            ImportDirectory.ImportDate Import = new ImportDirectory.ImportDate();
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

    private void LoadCLRDirectory()
    {
        DirAttrib clrRVA = (DirAttrib)_OptionalDirAttrib.DirByte[14];
        long address = GetLong(clrRVA.DirRva); //获取的位置
        if (address == 0) return;
        PEFileIndex = Rva2Fov(address);
        clrDirectory = new CLRDirectory();
        LoadBytesByType(clrDirectory);

        address = GetLong(clrDirectory.metaDir.DirRva); //获取的位置
        PEFileIndex = Rva2Fov(address);
        var metadataHeader = clrDirectory.metadataHeader = new MetadataHeader();
        LoadBytesByType(metadataHeader);
        for (int i = 0; i < metadataHeader.streamLength; i++)
        {
            HeapStream heapStream = new HeapStream();
            LoadBytesByType(heapStream);
            PEFileIndex = PEFileIndex - heapStream.name.Length + ((heapStream.name.Length + 1 + 3) & ~3U);
            metadataHeader.streams.Add(heapStream);
        }

        foreach (var stream in metadataHeader.streams)
        {
            var startPos = Rva2Fov(address) + stream.offset;
            PEFileIndex = startPos;
            stream.reader = new BytesArray(PEFileByte, (int)PEFileIndex, stream.streamSize);
            switch (stream.name)
            {
                case "#~":
                case "#-":
                    TableStreamHeader tableStreamHeader = new TableStreamHeader();
                    LoadBytesByType(tableStreamHeader);

                    TableStream tableStream = new TableStream();
                    var tableInfos = tableStream.CreateTables(tableStreamHeader.majorVersion,
                        tableStreamHeader.minorVersion, out int maxPresentTables);
                    var mdTables = new MDTable[tableInfos.Length];
                    var sizes = new uint[64];
                    var valid = tableStreamHeader.valid;

                    for (int i = 0; i < 64; valid >>= 1, i++)
                    {
                        uint rows = (valid & 1) == 0 ? 0 : ReadUInt32();
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
                        table.UpdateSize(sizes, ref offset);
                    }

                    tableStream.reader = new BytesArray(PEFileByte, (int)startPos, stream.streamSize);
                    tableStream.tablesPos = (uint)(PEFileIndex - startPos);
                    tableStream.tables = mdTables;
                    tableStream.InitializeTables();
                    metadataHeader.tableStream = tableStream;
                    break;
                case "#Strings":
                    metadataHeader.StringHeapStream = stream;
                    ReadStringList(stream);
                    break;
                case "#US":
                    metadataHeader.UsHeapStream = stream;
                    ReadCompressedStringList(stream);
                    break;
                case "#Blob":
                    metadataHeader.BlobHeapStream = stream;
                    ReadCompressedStringList(stream);
                    break;
                case "#GUID":
                    metadataHeader.GuidHeapStream = stream;
                    ReadGuid(stream);
                    break;
                case "#Pdb":
                    metadataHeader.PdbHeapStream = stream;
                    // ReadCompressedStringList(stream);
                    break;
                case "#JTD":
                    break;
            }
        }
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
        var offset = 0;
        while (true)
        {
            int length = stream.ReadCompressedUint32(out int lengthSize);
            if (!stream.IsValid())
                break;
            HeapStreamString value = new HeapStreamString();
            value.length = (int)(length / 2);
            value.offset = offset;
            value.value = stream.Loadbyte(length);
            stream.list.Add(value);
            offset += (int)length;
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

    private byte ReadInt8()
    {
        return Loadbyte(1)[0];
    }

    private short ReadInt16()
    {
        return BitConverter.ToInt16(Loadbyte(2), 0);
    }

    private int ReadInt32()
    {
        return BitConverter.ToInt32(Loadbyte(4), 0);
    }
    
    private ushort ReadUInt16()
    {
        return BitConverter.ToUInt16(Loadbyte(2), 0);
    }

    private uint ReadUInt32()
    {
        return BitConverter.ToUInt32(Loadbyte(4), 0);
    }

    private long ReadInt64()
    {
        return BitConverter.ToInt64(Loadbyte(8), 0);
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
                return Date;
            }

            ReadIndex++;
        }

        return new byte[0];
    }

    private string ReadString(int length = 0)
    {
        byte[] Date;
        if (length > 0)
        {
            Date = new byte[length];
            Loadbyte(ref Date);
        }
        else
        {
            Date = LoadAsciiName();
            if (Date == null)
                return "";
        }

        return Encoding.UTF8.GetString(Date);
    }

    #region Table

    #endregion

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
            if (Data.Length == 0)
                return "";
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

    public SectionData GetSectionDataByRVA(long rva)
    {
        for (int i = 0; i != _SectionTable.Section.Count; i++) //循环节表
        {
            SectionData Sect = (SectionData)_SectionTable.Section[i];

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
            DirAttrib MyDirByte = (DirAttrib)_OptionalDirAttrib.DirByte[i];
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
            SectionData SectionDate = (SectionData)_SectionTable.Section[k];
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