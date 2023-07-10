最近在看hybridclr源码，发现对PE格式不太了解，在网上查查资料，发现PE格式大有学问，于是打算写个工具解析它，边学边实践！选择用C#来实现，winform还是很好用的哈！

PE文件是Windows操作系统下使用的一种可执行文件，由COFF（UNIX平台下的通用对象文件格式）格式文件发展而来。32位成为PE32，64位称为PE+或PE32+。

## Dos头

```
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
```

一共64个字节，主要看看e_lfanew PE头的偏移就行！Dos头的下面是Dos存根，运行下dos命令中的代码和数据

Dos头+Dos存根是从 MS-DOS 2.0 兼容 EXE 标头到 PE 标头之前未使用的分区的部分是 MS-DOS 2.0 部分，仅用于 MS-DOS 兼容性

下面看看notepad.exe的信息

![img](https://pic1.zhimg.com/80/v2-7a655b7ea9b41eda04c547750a423f44_720w.png?source=d16d100b)



编辑切换为居中

添加图片注释，不超过 140 字（可选）

![img](https://picx.zhimg.com/80/v2-465e65239096d4ddff05bf0b62659d20_720w.png?source=d16d100b)



编辑切换为居中

添加图片注释，不超过 140 字（可选）

标红的地址0x3c就是PE头的偏移，指向,5045表示PE签名

## PE头

```
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
```

![img](https://picx.zhimg.com/80/v2-0fa9dd132e8db6b684c3dd2ab720d4bf_720w.png?source=d16d100b)



编辑切换为居中

添加图片注释，不超过 140 字（可选）

SizeOfOptionalHeader：表示可选头的大小

## 可选头

```
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
```

可选的标头 magic 号确定图像是 PE32 还是 PE32+ 可执行文件。从可选头开始，分64位和32位，读取的字段大小也不一样

| Magic 数字 | PE 格式 |
| ---------- | ------- |
| 0x10b      | PE32    |
| 0x20b      | PE32+   |

PE32+ 图像允许 64 位地址空间，同时将图像大小限制为 2 GB。 其他 PE32+ 修改在各自的部分中得到解决。可选标头本身有三个主要部分。

| 偏移量 (PE32/PE32+) | 大小 (PE32/PE32+) | 标头部件          | 说明                                                         |
| ------------------- | ----------------- | ----------------- | ------------------------------------------------------------ |
| 0                   | 28/24             | 标准字段          | 为 COFF 的所有实现定义的字段，包括UNIX。                     |
| 28/24               | 68/88             | 特定于Windows字段 | 支持Windows (的特定功能的其他字段，例如子系统) 。            |
| 96/112              | 变量              | 数据目录          | 映像文件中找到的特殊表的地址/大小对，由操作系统 (使用，例如导入表和导出表) 。 |

AddressOfEntryPoint 最开始运行的代码地址

ImageBase 装载内存后的虚拟基础地址，exe一般起始地址0x00400000，dll一般起始地址0x01000000

SectionAlignment 一般值为0x1000 ，FileAlignment 一般值为 0x200 ,用一图来表示

![img](https://picx.zhimg.com/80/v2-87737b2e665676eddb8096f316522928_720w.png?source=d16d100b)



编辑切换为居中

添加图片注释，不超过 140 字（可选）

加载内存后，各个节区是SectionAlignment 的倍数，在硬盘中，各个节区是FileAlignment 的倍数

## 数据目录表

OptionalHeader.NumberOfRvaAndSizes 目录的数量，一般为16

```
public class DirAttrib
{
    [HeadAttribute("内存中的地址", 4)] public byte[] DirRva;
    [HeadAttribute("内存中的大小", 4)] public byte[] DirSize;
}
```

![img](https://picx.zhimg.com/80/v2-19acb880b03cfb2b677d950fe32fcfb0_720w.png?source=d16d100b)



编辑切换为居中

添加图片注释，不超过 140 字（可选）

## 节表信息

PEHeader.NumberOfSections 节的数量

```
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
```

![img](https://picx.zhimg.com/80/v2-cf06efbf77bed405a887116cb5cfd129_720w.png?source=d16d100b)



编辑切换为居中

添加图片注释，不超过 140 字（可选）

可以看出VirtualSize(内存中的节的大小) 和 SizeOfRawData(磁盘文件中节的大小)大小不一样

FOA (File Ofseet Address，文件偏移地址) 

RVA（Relative Virtual Address，相对虚拟地址）

RAW to FOV 的公式如下

```
FOV - PointerToRawData = RVA - VirtualAddress 
FOV = RVA - VirtualAddress + PointerToRawData 
```

这个概念需要好好理解一下，参考资料[PE格式第三讲扩展,VA,RVA,FA(RAW),模块地址的概念 - iBinary - 博客园](https://www.cnblogs.com/ibinary/p/7653693.html)

导出表，顾名思义就是导出给其他程序调用

![img](https://picx.zhimg.com/80/v2-3089a209b6b1734fc9fafdcb704b38e5_720w.png?source=d16d100b)



编辑切换为居中

添加图片注释，不超过 140 字（可选）

导入表，引用一些库函数

![img](https://picx.zhimg.com/80/v2-9480a1951f056170e8dc527021871ca6_720w.png?source=d16d100b)



编辑切换为居中

添加图片注释，不超过 140 字（可选）

工作中经常使用unity,CLR也必须也需要解析下，本以为不会花太多时间，毕竟PE格式(表面上)没花太多时间，可在CLR解析上，花费了大量的时间，CLR部分比较复杂，参考资料也不是很多，最后还是剩下签名部分没有处理(累了，躺平了，有时间再补上吧)

```
    public static TableStream tableStream;
    public static HeapStream StringHeapStream;
    public static HeapStream UsHeapStream;
    public static HeapStream BlobHeapStream;
    public static HeapStream GuidHeapStream;
    public static HeapStream PdbHeapStream;
```

CLR主要是这几个流的解析

TableStream流十分复杂，有50个表类型

高2位表示类型，后面变式rid

```
public enum Table : byte
{
    /// <summary>Module table (00h)</summary>
    Module,

    /// <summary>TypeRef table (01h)</summary>
    TypeRef,

    /// <summary>TypeDef table (02h)</summary>
    TypeDef,

    /// <summary>FieldPtr table (03h)</summary>
    FieldPtr,

    /// <summary>Field table (04h)</summary>
    Field,

    /// <summary>MethodPtr table (05h)</summary>
    MethodPtr,

    /// <summary>Method table (06h)</summary>
    Method,

    /// <summary>ParamPtr table (07h)</summary>
    ParamPtr,

    /// <summary>Param table (08h)</summary>
    Param,

    /// <summary>InterfaceImpl table (09h)</summary>
    InterfaceImpl,

    /// <summary>MemberRef table (0Ah)</summary>
    MemberRef,

    /// <summary>Constant table (0Bh)</summary>
    Constant,

    /// <summary>CustomAttribute table (0Ch)</summary>
    CustomAttribute,

    /// <summary>FieldMarshal table (0Dh)</summary>
    FieldMarshal,

    /// <summary>DeclSecurity table (0Eh)</summary>
    DeclSecurity,

    /// <summary>ClassLayout table (0Fh)</summary>
    ClassLayout,

    /// <summary>FieldLayout table (10h)</summary>
    FieldLayout,

    /// <summary>StandAloneSig table (11h)</summary>
    StandAloneSig,

    /// <summary>EventMap table (12h)</summary>
    EventMap,

    /// <summary>EventPtr table (13h)</summary>
    EventPtr,

    /// <summary>Event table (14h)</summary>
    Event,

    /// <summary>PropertyMap table (15h)</summary>
    PropertyMap,

    /// <summary>PropertyPtr table (16h)</summary>
    PropertyPtr,

    /// <summary>Property table (17h)</summary>
    Property,

    /// <summary>MethodSemantics table (18h)</summary>
    MethodSemantics,

    /// <summary>MethodImpl table (19h)</summary>
    MethodImpl,

    /// <summary>ModuleRef table (1Ah)</summary>
    ModuleRef,

    /// <summary>TypeSpec table (1Bh)</summary>
    TypeSpec,

    /// <summary>ImplMap table (1Ch)</summary>
    ImplMap,

    /// <summary>FieldRVA table (1Dh)</summary>
    FieldRVA,

    /// <summary>ENCLog table (1Eh)</summary>
    ENCLog,

    /// <summary>ENCMap table (1Fh)</summary>
    ENCMap,

    /// <summary>Assembly table (20h)</summary>
    Assembly,

    /// <summary>AssemblyProcessor table (21h)</summary>
    AssemblyProcessor,

    /// <summary>AssemblyOS table (22h)</summary>
    AssemblyOS,

    /// <summary>AssemblyRef table (23h)</summary>
    AssemblyRef,

    /// <summary>AssemblyRefProcessor table (24h)</summary>
    AssemblyRefProcessor,

    /// <summary>AssemblyRefOS table (25h)</summary>
    AssemblyRefOS,

    /// <summary>File table (26h)</summary>
    File,

    /// <summary>ExportedType table (27h)</summary>
    ExportedType,

    /// <summary>ManifestResource table (28h)</summary>
    ManifestResource,

    /// <summary>NestedClass table (29h)</summary>
    NestedClass,

    /// <summary>GenericParam table (2Ah)</summary>
    GenericParam,

    /// <summary>MethodSpec table (2Bh)</summary>
    MethodSpec,

    /// <summary>GenericParamConstraint table (2Ch)</summary>
    GenericParamConstraint,

    /// <summary>(Portable PDB) Document table (30h)</summary>
    Document = 0x30,

    /// <summary>(Portable PDB) MethodDebugInformation table (31h)</summary>
    MethodDebugInformation,

    /// <summary>(Portable PDB) LocalScope table (32h)</summary>
    LocalScope,

    /// <summary>(Portable PDB) LocalVariable table (33h)</summary>
    LocalVariable,

    /// <summary>(Portable PDB) LocalConstant table (34h)</summary>
    LocalConstant,

    /// <summary>(Portable PDB) ImportScope table (35h)</summary>
    ImportScope,

    /// <summary>(Portable PDB) StateMachineMethod table (36h)</summary>
    StateMachineMethod,

    /// <summary>(Portable PDB) CustomDebugInformation table (37h)</summary>
    CustomDebugInformation,
}
```

![img](https://picx.zhimg.com/80/v2-fe709c7eb9244e184669b95f3952c243_720w.png?source=d16d100b)





添加图片注释，不超过 140 字（可选）

![img](https://pic1.zhimg.com/80/v2-10f0054aec82fb51ce3fc2af0bee5e50_720w.png?source=d16d100b)



编辑

添加图片注释，不超过 140 字（可选）

![img](https://pic1.zhimg.com/80/v2-a756bc81c030345a1fdd3b50d36ba28d_720w.png?source=d16d100b)



编辑切换为居中

添加图片注释，不超过 140 字（可选）

![img](https://picx.zhimg.com/80/v2-51d3d543831fe4bcae4773fdd73faee8_720w.png?source=d16d100b)



编辑切换为居中

添加图片注释，不超过 140 字（可选）

\#String 主要是系统用的，一些方法名字，类名，资源名称等等

![img](https://pic1.zhimg.com/80/v2-6f2542572ee188c54e5d7b13315e0ad4_720w.png?source=d16d100b)



编辑

添加图片注释，不超过 140 字（可选）

\#US 主要是用户定义

![img](https://pic1.zhimg.com/80/v2-95df5b17d46674e008340e78914a33ab_720w.png?source=d16d100b)



编辑

添加图片注释，不超过 140 字（可选）

\#Blob 二进制数据

![img](https://pica.zhimg.com/80/v2-d2d902a345ee8c2227e2a4b26104f190_720w.png?source=d16d100b)



编辑切换为居中

添加图片注释，不超过 140 字（可选）

然后就是IL指令解析

![img](https://pic1.zhimg.com/80/v2-b4d8a0208f564b09b6acc90b82546ae4_720w.png?source=d16d100b)



编辑切换为居中

添加图片注释，不超过 140 字（可选）

项目源码地址:

https://github.com/wujuju/PE-Tools.gitgithub.com/wujuju/PE-Tools.git

源码主要参考：

1.

GitHub - focus-creative-games/hybridclr: HybridCLR是一个特性完整、零成本、高性能、低内存的Unity全平台原生c#热更方案。 HybridCLR is a fully featured, zero-cost, high-performance, low-memory solution for Unity's all-platform native c# hotupdate.github.com/focus-creative-games/hybridclr

2.

GitHub - 0xd4d/dnlib: Reads and writes .NET assemblies and modulesgithub.com/0xd4d/dnlib

总结：了解PE格式的一些好处

1，能比较好的理解window系统如何运行一个程序程序

2，PE头加密，解压，了解一些壳的运行机制

3，更好的掌握逆向分析，让破解增加难度

4，深入了解CLR

把自己理解的东西写出来真的需要花费很多时间，这篇其实写的很马虎，有很多东西可以深入讲一讲，工作太忙了，自己精力也有限，大家直接看源码吧
