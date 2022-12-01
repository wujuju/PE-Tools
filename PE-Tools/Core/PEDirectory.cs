using System.Collections.Generic;

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
    }