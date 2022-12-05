public class PEInfoType
{
    public enum EnMachine
    {
        MACHINE_UNKNOWN = 0x0, //假定此字段的内容适用于任何计算机类型
        MACHINE_AM33 = 0x1d3, //松田 AM33
        MACHINE_AMD64 = 0x8664, //X64
        MACHINE_ARM = 0x1c0, //ARM 小 endian
        MACHINE_ARM64 = 0xaa64, //ARM64 小 endian
        MACHINE_ARMNT = 0x1c4, //ARM Thumb-2 小 endian
        MACHINE_EBC = 0xebc, //EFI 字节代码
        MACHINE_I386 = 0x14c, //Intel 386 或更高版本的处理器和兼容的处理器
        MACHINE_IA64 = 0x200, //Intel Itanium 处理器系列
        MACHINE_LOONGARCH32 = 0x6232, //LoongArch 32 位处理器系列
        MACHINE_LOONGARCH64 = 0x6264, //LoongArch 64 位处理器系列
        MACHINE_M32R = 0x9041, //三菱M32R小尾声
        MACHINE_MIPS16 = 0x266, //MIPS16
        MACHINE_MIPSFPU = 0x366, //使用 FPU 的 MIPS
        MACHINE_MIPSFPU16 = 0x466, //具有 FPU 的 MIPS16
        MACHINE_POWERPC = 0x1f0, //Power PC 小 endian
        MACHINE_POWERPCFP = 0x1f1, //支持浮点支持的电源电脑
        MACHINE_R4000 = 0x166, //MIPS 小 endian
        MACHINE_RISCV32 = 0x5032, //RISC - V 32 位地址空间
        MACHINE_RISCV64 = 0x5064, //RISC - V 64 位地址空间
        MACHINE_RISCV128 = 0x5128, //RISC - V 128 位地址空间
        MACHINE_SH3 = 0x1a2, //Hitachi SH3
        MACHINE_SH3DSP = 0x1a3, //Hitachi SH3 DSP
        MACHINE_SH4 = 0x1a6, //Hitachi SH4
        MACHINE_SH5 = 0x1a8, //Hitachi SH5
        MACHINE_THUMB = 0x1c2, //Thumb
        MACHINE_WCEMIPSV2 = 0x169, //MIPS 小端 WCE v2
    }

    public enum EnImageCharacteristic
    {
        RELOCS_STRIPPED =
            0x0001, //仅映像、Windows CE和 Microsoft Windows NT 及更高版本。 这表示该文件不包含基本重定位，因此必须加载在其首选基址处。 如果基址不可用，加载程序将报告错误。 链接器的默认行为是从可执行文件 (EXE) 文件中去除基本重定位。
        EXECUTABLE_IMAGE = 0x0002, //仅图像。 这表示映像文件有效且可以运行。 如果未设置此标志，则表示链接器错误。
        LINE_NUMS_STRIPPED = 0x0004, //COFF 行号已删除。 此标志已弃用，应为零。
        LOCAL_SYMS_STRIPPED = 0x0008, //已删除本地符号的 COFF 符号表条目。 此标志已弃用，应为零。
        AGGRESSIVE_WS_TRIM = 0x0010, //已过时。 积极剪裁工作集。 此标志已弃用 Windows 2000 及更高版本，并且必须为零。
        LARGE_ADDRESS_AWARE = 0x0020, //应用程序可以处理 > 2 GB 地址。
        NONE = 0x0040, //此标志保留供将来使用。
        BYTES_REVERSED_LO = 0x0080, //小尾数：LSB) 最不重要的位 (LSB) 位于内存中最重要的位 (MSB) 。 此标志已弃用，应为零。
        E32BIT_MACHINE = 0x0100, //计算机基于 32 位体系结构。
        DEBUG_STRIPPED = 0x0200, //从映像文件中删除调试信息。
        REMOVABLE_RUN_FROM_SWAP = 0x0400, //如果映像位于可移动媒体上，请完全加载该映像并将其复制到交换文件。
        NET_RUN_FROM_SWAP = 0x0800, //如果映像位于网络媒体上，请完全加载映像并将其复制到交换文件。
        SYSTEM = 0x1000, //映像文件是系统文件，而不是用户程序。
        DLL = 0x2000, //映像文件是动态链接库 (DLL) 。 此类文件被视为几乎所有目的的可执行文件，尽管无法直接运行这些文件。
        UP_SYSTEM_ONLY = 0x4000, //该文件应仅在单处理器计算机上运行。
        BYTES_REVERSED_HI = 0x8000, //大尾号：MSB 位于内存中的 LSB 之前。 此标志已弃用，应为零。
    }

    public enum EnMagic
    {
        PE32 = 0x10b,
        PE32Add = 0x20b,
    }

    public enum EnSubsystem
    {
        UNKNOWN = 0, //未知子系统
        NATIVE = 1, //设备驱动程序和本机Windows进程
        WINDOWS_GUI = 2, //Windows图形用户界面 (GUI) 子系统
        WINDOWS_CUI = 3, //Windows字符子系统
        OS2_CUI = 5, //OS/2 字符子系统
        POSIX_CUI = 7, //Posix 字符子系统
        NATIVE_WINDOWS = 8, //本机 Win9x 驱动程序
        WINDOWS_CE_GUI = 9, //Windows CE
        EFI_APPLICATION = 10, //可扩展固件接口 (EFI) 应用程序
        EFI_BOOT_SERVICE_DRIVER = 11, //具有启动服务的 EFI 驱动程序
        EFI_RUNTIME_DRIVER = 12, //具有运行时服务的 EFI 驱动程序
        EFI_ROM = 13, //EFI ROM 映像
        XBOX = 14, //XBOX
        WINDOWS_BOOT_APPLICATION = 16, //Windows启动应用程序
    }

    public enum EnDLLCharacteristic
    {
        HIGH_ENTROPY_VA = 0x0020, //图像可以处理高萎缩 64 位虚拟地址空间。
        DYNAMIC_BASE = 0x0040, //DLL 可以在加载时重新定位。
        FORCE_INTEGRITY = 0x0080, //强制实施代码完整性检查
        NX_COMPAT = 0x0100, //映像与 NX 兼容。
        NO_ISOLATION = 0x0200, //隔离感知，但不隔离映像。
        NO_SEH = 0x0400, //不使用结构化异常 (标准版) 处理。 此映像中无法调用任何标准版处理程序。
        NO_BIND = 0x0800, //不要绑定映像。
        APPCONTAINER = 0x1000, //映像必须在 AppContainer 中执行。
        WDM_DRIVER = 0x2000, //WDM 驱动程序。
        GUARD_CF = 0x4000, //映像支持 Control Flow Guard。
        TERMINAL_SERVER_AWARE = 0x8000, //终端服务器感知
    }
}

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

public enum ColumnSize : byte {
		None,
		/// <summary>RID into Module table</summary>
		Module,
		/// <summary>RID into TypeRef table</summary>
		TypeRef,
		/// <summary>RID into TypeDef table</summary>
		TypeDef,
		/// <summary>RID into FieldPtr table</summary>
		FieldPtr,
		/// <summary>RID into Field table</summary>
		Field,
		FieldList,
		/// <summary>RID into MethodPtr table</summary>
		MethodPtr,
		/// <summary>RID into Method table</summary>
		Method,
		MethodList,
		/// <summary>RID into ParamPtr table</summary>
		ParamPtr,
		/// <summary>RID into Param table</summary>
		Param,
		/// <summary>RID into InterfaceImpl table</summary>
		InterfaceImpl,
		/// <summary>RID into MemberRef table</summary>
		MemberRef,
		/// <summary>RID into Constant table</summary>
		Constant,
		/// <summary>RID into CustomAttribute table</summary>
		CustomAttribute,
		/// <summary>RID into FieldMarshal table</summary>
		FieldMarshal,
		/// <summary>RID into DeclSecurity table</summary>
		DeclSecurity,
		/// <summary>RID into ClassLayout table</summary>
		ClassLayout,
		/// <summary>RID into FieldLayout table</summary>
		FieldLayout,
		/// <summary>RID into StandAloneSig table</summary>
		StandAloneSig,
		/// <summary>RID into EventMap table</summary>
		EventMap,
		/// <summary>RID into EventPtr table</summary>
		EventPtr,
		/// <summary>RID into Event table</summary>
		Event,
		/// <summary>RID into PropertyMap table</summary>
		PropertyMap,
		/// <summary>RID into PropertyPtr table</summary>
		PropertyPtr,
		/// <summary>RID into Property table</summary>
		Property,
		/// <summary>RID into MethodSemantics table</summary>
		MethodSemantics,
		/// <summary>RID into MethodImpl table</summary>
		MethodImpl,
		/// <summary>RID into ModuleRef table</summary>
		ModuleRef,
		/// <summary>RID into TypeSpec table</summary>
		TypeSpec,
		/// <summary>RID into ImplMap table</summary>
		ImplMap,
		/// <summary>RID into FieldRVA table</summary>
		FieldRVA,
		/// <summary>RID into ENCLog table</summary>
		ENCLog,
		/// <summary>RID into ENCMap table</summary>
		ENCMap,
		/// <summary>RID into Assembly table</summary>
		Assembly,
		/// <summary>RID into AssemblyProcessor table</summary>
		AssemblyProcessor,
		/// <summary>RID into AssemblyOS table</summary>
		AssemblyOS,
		/// <summary>RID into AssemblyRef table</summary>
		AssemblyRef,
		/// <summary>RID into AssemblyRefProcessor table</summary>
		AssemblyRefProcessor,
		/// <summary>RID into AssemblyRefOS table</summary>
		AssemblyRefOS,
		/// <summary>RID into File table</summary>
		File,
		/// <summary>RID into ExportedType table</summary>
		ExportedType,
		/// <summary>RID into ManifestResource table</summary>
		ManifestResource,
		/// <summary>RID into NestedClass table</summary>
		NestedClass,
		/// <summary>RID into GenericParam table</summary>
		GenericParam,
		/// <summary>RID into MethodSpec table</summary>
		MethodSpec,
		/// <summary>RID into GenericParamConstraint table</summary>
		GenericParamConstraint,
		/// <summary>RID into Document table</summary>
		Document = 0x30,
		/// <summary>RID into MethodDebugInformation table</summary>
		MethodDebugInformation,
		/// <summary>RID into LocalScope table</summary>
		LocalScope,
		/// <summary>RID into LocalVariable table</summary>
		LocalVariable,
		/// <summary>RID into LocalConstant table</summary>
		LocalConstant,
		/// <summary>RID into ImportScope table</summary>
		ImportScope,
		/// <summary>RID into StateMachineMethod table</summary>
		StateMachineMethod,
		/// <summary>RID into CustomDebugInformation table</summary>
		CustomDebugInformation,
		/// <summary>8-bit byte</summary>
		Byte = 0x40,
		/// <summary>16-bit signed int</summary>
		Int16,
		/// <summary>16-bit unsigned int</summary>
		UInt16,
		/// <summary>32-bit signed int</summary>
		Int32,
		/// <summary>32-bit unsigned int</summary>
		UInt32,
		TypeFlags,
		FieldFlags,
		MethodFlags,
		/// <summary>Index into #Strings stream</summary>
		Strings,
		/// <summary>Index into #GUID stream</summary>
		GUID,
		/// <summary>Index into #Blob stream</summary>
		Blob,
		/// <summary>TypeDefOrRef encoded token</summary>
		TypeDefOrRef,
		/// <summary>HasConstant encoded token</summary>
		HasConstant,
		/// <summary>HasCustomAttribute encoded token</summary>
		HasCustomAttribute,
		/// <summary>HasFieldMarshal encoded token</summary>
		HasFieldMarshal,
		/// <summary>HasDeclSecurity encoded token</summary>
		HasDeclSecurity,
		/// <summary>MemberRefParent encoded token</summary>
		MemberRefParent,
		/// <summary>HasSemantic encoded token</summary>
		HasSemantic,
		/// <summary>MethodDefOrRef encoded token</summary>
		MethodDefOrRef,
		/// <summary>MemberForwarded encoded token</summary>
		MemberForwarded,
		/// <summary>Implementation encoded token</summary>
		Implementation,
		/// <summary>CustomAttributeType encoded token</summary>
		CustomAttributeType,
		/// <summary>ResolutionScope encoded token</summary>
		ResolutionScope,
		/// <summary>TypeOrMethodDef encoded token</summary>
		TypeOrMethodDef,
		/// <summary>HasCustomDebugInformation encoded token</summary>
		HasCustomDebugInformation,
	}