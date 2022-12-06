using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

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

public class DosStub
{
    public byte[] DosStubData;

    public DosStub(long Size)
    {
        DosStubData = new byte[Size];
    }
}

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
    public List<DirAttrib> DirByte = new List<DirAttrib>();
}

/// <summary>
/// 节表
/// </summary>
public class SectionTable
{
    public List<SectionData> Section = new List<SectionData>();
}

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

public class DirAttrib
{
    [HeadAttribute("内存中的地址", 4)] public byte[] DirRva;
    [HeadAttribute("内存中的大小", 4)] public byte[] DirSize;
}
