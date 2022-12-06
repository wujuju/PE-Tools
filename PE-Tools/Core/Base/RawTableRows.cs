using System;
using System.Collections.Generic;

/// <summary>
/// Raw contents of an uncompressed Module table row
/// </summary>
public class RawModuleRow
{
    public readonly ushort Generation;
    public readonly uint Name;
    public readonly uint Mvid;
    public readonly uint EncId;
    public readonly uint EncBaseId;

    public RawModuleRow(ushort Generation, uint Name, uint Mvid, uint EncId, uint EncBaseId)
    {
        this.Generation = Generation;
        this.Name = Name;
        this.Mvid = Mvid;
        this.EncId = EncId;
        this.EncBaseId = EncBaseId;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Generation,
            1 => Name,
            2 => Mvid,
            3 => EncId,
            4 => EncBaseId,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed TypeRef table row
/// </summary>
public class RawTypeRefRow
{
    public readonly uint ResolutionScope;
    public readonly uint Name;
    public readonly uint Namespace;

    public RawTypeRefRow(uint ResolutionScope, uint Name, uint Namespace)
    {
        this.ResolutionScope = ResolutionScope;
        this.Name = Name;
        this.Namespace = Namespace;
        SetMetadataHeader();
    }

    public string name;
    public string nameSpace;

    void SetMetadataHeader()
    {
        name = MetadataHeader.StringHeapStream.Read(Name);
        nameSpace = MetadataHeader.StringHeapStream.Read(Namespace);
    }

    public override string ToString()
    {
        if (nameSpace.Length == 0)
            return name;
        return String.Format("[{1}][{0}]", name, nameSpace);
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => ResolutionScope,
            1 => Name,
            2 => Namespace,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed TypeDef table row
/// </summary>
public class RawTypeDefRow
{
    public readonly uint Rid;
    public readonly uint Flags;
    public readonly uint Name;
    public readonly uint Namespace;
    public readonly uint Extends;
    public readonly uint FieldList;
    public readonly uint MethodList;
    public List<RawFieldRow> fieldRidList = new List<RawFieldRow>();
    public List<RawMethodRow> methodRidList = new List<RawMethodRow>();

    public RawTypeDefRow(uint Rid, uint Flags, uint Name, uint Namespace, uint Extends, uint FieldList, uint MethodList)
    {
        this.Rid = Rid;
        this.Flags = Flags;
        this.Name = Name;
        this.Namespace = Namespace;
        this.Extends = Extends;
        this.FieldList = FieldList;
        this.MethodList = MethodList;
        SetMetadataHeader();
    }

    public string name;
    public string nameSpace;

    void SetMetadataHeader()
    {
        var tmpFieldRidList = MetadataHeader.tableStream.GetFieldRidList(Rid);
        var tmpMethodRidList = MetadataHeader.tableStream.GetMethodRidList(Rid);
        name = MetadataHeader.StringHeapStream.Read(Name);
        nameSpace = MetadataHeader.StringHeapStream.Read(Namespace);

        for (int i = 0; i < tmpFieldRidList.Count; i++)
        {
            var field = MetadataHeader.tableStream.ResolveField(tmpFieldRidList[i]);
            field.typeDefRow = this;
            fieldRidList.Add(field);
        }

        for (int i = 0; i < tmpMethodRidList.Count; i++)
        {
            var method = MetadataHeader.tableStream.ResolveMethod(tmpMethodRidList[i]);
            method.typeDefRow = this;
            methodRidList.Add(method);
        }
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Flags,
            1 => Name,
            2 => Namespace,
            3 => Extends,
            4 => FieldList,
            5 => MethodList,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed FieldPtr table row
/// </summary>
public class RawFieldPtrRow
{
    public readonly uint Field;

    public RawFieldPtrRow(uint Field) => this.Field = Field;

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Field,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed Field table row
/// </summary>
public class RawFieldRow
{
    public readonly ushort Flags;
    public readonly uint Name;
    public readonly uint Signature;
    public RidList paramRidList;

    public RawFieldRow(ushort Flags, uint Name, uint Signature)
    {
        this.Flags = Flags;
        this.Name = Name;
        this.Signature = Signature;
        SetMetadataHeader();
    }

    public string name;
    public RawTypeDefRow typeDefRow;

    void SetMetadataHeader()
    {
        name = MetadataHeader.StringHeapStream.Read(Name);
    }

    public override string ToString()
    {
        return string.Format("[{0}]{1}", typeDefRow.name, name);
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Flags,
            1 => Name,
            2 => Signature,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed MethodPtr table row
/// </summary>
public class RawMethodPtrRow
{
    public readonly uint Method;

    public RawMethodPtrRow(uint Method) => this.Method = Method;

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Method,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed Method table row
/// </summary>
public class RawMethodRow
{
    public readonly uint Rid;
    public readonly uint RVA;
    public readonly ushort ImplFlags;
    public readonly ushort Flags;
    public readonly uint Name;
    public readonly uint Signature;
    public readonly uint ParamList;

    public RawMethodRow(uint Rid, uint RVA, ushort ImplFlags, ushort Flags, uint Name, uint Signature, uint ParamList)
    {
        this.Rid = Rid;
        this.RVA = RVA;
        this.ImplFlags = ImplFlags;
        this.Flags = Flags;
        this.Name = Name;
        this.Signature = Signature;
        this.ParamList = ParamList;
        SetMetadataHeader();
    }

    public string name;
    public RawTypeDefRow typeDefRow;

    void SetMetadataHeader()
    {
        name = MetadataHeader.StringHeapStream.Read(Name);
        var tmpParamRidList = MetadataHeader.tableStream.GetParamRidList(Rid);

        for (int i = 0; i < tmpParamRidList.Count; i++)
        {
            var param = MetadataHeader.tableStream.ResolveParam(tmpParamRidList[i]);
            param.methodRow = this;
        }
    }

    public override string ToString()
    {
        return string.Format("[{0}]{1}", typeDefRow.name, name);
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => RVA,
            1 => ImplFlags,
            2 => Flags,
            3 => Name,
            4 => Signature,
            5 => ParamList,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed ParamPtr table row
/// </summary>
public class RawParamPtrRow
{
    public readonly uint Param;

    public RawParamPtrRow(uint Param) => this.Param = Param;

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Param,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed Param table row
/// </summary>
public class RawParamRow
{
    public readonly ushort Flags;
    public readonly ushort Sequence;
    public readonly uint Name;

    public RawParamRow(ushort Flags, ushort Sequence, uint Name)
    {
        this.Flags = Flags;
        this.Sequence = Sequence;
        this.Name = Name;
        SetMetadataHeader();
    }


    public string name;
    public RawMethodRow methodRow;

    void SetMetadataHeader()
    {
        name = MetadataHeader.StringHeapStream.Read(Name);
    }

    public override string ToString()
    {
        return methodRow.ToString() + name;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Flags,
            1 => Sequence,
            2 => Name,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed InterfaceImpl table row
/// </summary>
public class RawInterfaceImplRow
{
    public readonly uint Class;
    public readonly uint Interface;

    public RawInterfaceImplRow(uint Class, uint Interface)
    {
        this.Class = Class;
        this.Interface = Interface;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Class,
            1 => Interface,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed MemberRef table row
/// </summary>
public class RawMemberRefRow
{
    public readonly uint Class;
    public readonly uint Name;
    public readonly uint Signature;

    public RawMemberRefRow(uint Class, uint Name, uint Signature)
    {
        this.Class = Class;
        this.Name = Name;
        this.Signature = Signature;
        SetMetadataHeader();
    }

    public string name;

    void SetMetadataHeader()
    {
        name = MetadataHeader.StringHeapStream.Read(Name);
    }

    public override string ToString()
    {
        var classObj = MetadataHeader.tableStream.ResolveMemberRefParent(Class);
        if (classObj != null)
            return string.Format("[{0}]{1}", classObj, name);
        return name;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Class,
            1 => Name,
            2 => Signature,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed Constant table row
/// </summary>
public class RawConstantRow
{
    public readonly byte Type;
    public readonly byte Padding;
    public readonly uint Parent;
    public readonly uint Value;

    public RawConstantRow(byte Type, byte Padding, uint Parent, uint Value)
    {
        this.Type = Type;
        this.Padding = Padding;
        this.Parent = Parent;
        this.Value = Value;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Type,
            1 => Padding,
            2 => Parent,
            3 => Value,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed CustomAttribute table row
/// </summary>
public class RawCustomAttributeRow
{
    public readonly uint Parent;
    public readonly uint Type;
    public readonly uint Value;

    public RawCustomAttributeRow(uint Parent, uint Type, uint Value)
    {
        this.Parent = Parent;
        this.Type = Type;
        this.Value = Value;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Parent,
            1 => Type,
            2 => Value,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed FieldMarshal table row
/// </summary>
public class RawFieldMarshalRow
{
    public readonly uint Parent;
    public readonly uint NativeType;

    public RawFieldMarshalRow(uint Parent, uint NativeType)
    {
        this.Parent = Parent;
        this.NativeType = NativeType;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Parent,
            1 => NativeType,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed DeclSecurity table row
/// </summary>
public class RawDeclSecurityRow
{
    public readonly short Action;
    public readonly uint Parent;
    public readonly uint PermissionSet;

    public RawDeclSecurityRow(short Action, uint Parent, uint PermissionSet)
    {
        this.Action = Action;
        this.Parent = Parent;
        this.PermissionSet = PermissionSet;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => (uint)(int)Action,
            1 => Parent,
            2 => PermissionSet,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed ClassLayout table row
/// </summary>
public class RawClassLayoutRow
{
    public readonly ushort PackingSize;
    public readonly uint ClassSize;
    public readonly uint Parent;

    public RawClassLayoutRow(ushort PackingSize, uint ClassSize, uint Parent)
    {
        this.PackingSize = PackingSize;
        this.ClassSize = ClassSize;
        this.Parent = Parent;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => PackingSize,
            1 => ClassSize,
            2 => Parent,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed FieldLayout table row
/// </summary>
public class RawFieldLayoutRow
{
    public readonly uint OffSet;
    public readonly uint Field;

    public RawFieldLayoutRow(uint OffSet, uint Field)
    {
        this.OffSet = OffSet;
        this.Field = Field;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => OffSet,
            1 => Field,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed StandAloneSig table row
/// </summary>
public class RawStandAloneSigRow
{
    public readonly uint Signature;

    public RawStandAloneSigRow(uint Signature) => this.Signature = Signature;

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Signature,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed EventMap table row
/// </summary>
public class RawEventMapRow
{
    public readonly uint Parent;
    public readonly uint EventList;

    public RawEventMapRow(uint Parent, uint EventList)
    {
        this.Parent = Parent;
        this.EventList = EventList;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Parent,
            1 => EventList,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed EventPtr table row
/// </summary>
public class RawEventPtrRow
{
    public readonly uint Event;

    public RawEventPtrRow(uint Event) => this.Event = Event;

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Event,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed Event table row
/// </summary>
public class RawEventRow
{
    public readonly ushort EventFlags;
    public readonly uint Name;
    public readonly uint EventType;

    public RawEventRow(ushort EventFlags, uint Name, uint EventType)
    {
        this.EventFlags = EventFlags;
        this.Name = Name;
        this.EventType = EventType;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => EventFlags,
            1 => Name,
            2 => EventType,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed PropertyMap table row
/// </summary>
public class RawPropertyMapRow
{
    public readonly uint Parent;
    public readonly uint PropertyList;

    public RawPropertyMapRow(uint Parent, uint PropertyList)
    {
        this.Parent = Parent;
        this.PropertyList = PropertyList;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Parent,
            1 => PropertyList,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed PropertyPtr table row
/// </summary>
public class RawPropertyPtrRow
{
    public readonly uint Property;

    public RawPropertyPtrRow(uint Property) => this.Property = Property;

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Property,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed Property table row
/// </summary>
public class RawPropertyRow
{
    public readonly ushort PropFlags;
    public readonly uint Name;
    public readonly uint Type;

    public RawPropertyRow(ushort PropFlags, uint Name, uint Type)
    {
        this.PropFlags = PropFlags;
        this.Name = Name;
        this.Type = Type;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => PropFlags,
            1 => Name,
            2 => Type,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed MethodSemantics table row
/// </summary>
public class RawMethodSemanticsRow
{
    public readonly ushort Semantic;
    public readonly uint Method;
    public readonly uint Association;

    public RawMethodSemanticsRow(ushort Semantic, uint Method, uint Association)
    {
        this.Semantic = Semantic;
        this.Method = Method;
        this.Association = Association;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Semantic,
            1 => Method,
            2 => Association,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed MethodImpl table row
/// </summary>
public class RawMethodImplRow
{
    public readonly uint Class;
    public readonly uint MethodBody;
    public readonly uint MethodDeclaration;

    public RawMethodImplRow(uint Class, uint MethodBody, uint MethodDeclaration)
    {
        this.Class = Class;
        this.MethodBody = MethodBody;
        this.MethodDeclaration = MethodDeclaration;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Class,
            1 => MethodBody,
            2 => MethodDeclaration,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed ModuleRef table row
/// </summary>
public class RawModuleRefRow
{
    public readonly uint Name;

    public RawModuleRefRow(uint Name) => this.Name = Name;

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Name,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed TypeSpec table row
/// </summary>
public class RawTypeSpecRow
{
    public readonly uint Signature;

    public RawTypeSpecRow(uint Signature) => this.Signature = Signature;
    private BytesArray mReader;

    public BytesArray Reader
    {
        get
        {
            if (mReader == null)
            {
                var stream = MetadataHeader.BlobHeapStream;
                stream.reader.Position = Signature;
                var length = stream.ReadCompressedUint32(out var lengthSize);
                mReader = new BytesArray(stream.reader, 0, length);
            }

            return mReader;
        }
    }

    public override string ToString()
    {
        MetadataHeader.tableStream.ReadTypeSignature(Reader);
        return String.Format("RawTypeSpecRow");
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Signature,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed ImplMap table row
/// </summary>
public class RawImplMapRow
{
    public readonly ushort MappingFlags;
    public readonly uint MemberForwarded;
    public readonly uint ImportName;
    public readonly uint ImportScope;

    public RawImplMapRow(ushort MappingFlags, uint MemberForwarded, uint ImportName, uint ImportScope)
    {
        this.MappingFlags = MappingFlags;
        this.MemberForwarded = MemberForwarded;
        this.ImportName = ImportName;
        this.ImportScope = ImportScope;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => MappingFlags,
            1 => MemberForwarded,
            2 => ImportName,
            3 => ImportScope,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed FieldRVA table row
/// </summary>
public class RawFieldRVARow
{
    public readonly uint RVA;
    public readonly uint Field;

    public RawFieldRVARow(uint RVA, uint Field)
    {
        this.RVA = RVA;
        this.Field = Field;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => RVA,
            1 => Field,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed ENCLog table row
/// </summary>
public class RawENCLogRow
{
    public readonly uint Token;
    public readonly uint FuncCode;

    public RawENCLogRow(uint Token, uint FuncCode)
    {
        this.Token = Token;
        this.FuncCode = FuncCode;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Token,
            1 => FuncCode,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed ENCMap table row
/// </summary>
public class RawENCMapRow
{
    public readonly uint Token;

    public RawENCMapRow(uint Token) => this.Token = Token;

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Token,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed Assembly table row
/// </summary>
public class RawAssemblyRow
{
    public readonly uint HashAlgId;
    public readonly ushort MajorVersion;
    public readonly ushort MinorVersion;
    public readonly ushort BuildNumber;
    public readonly ushort RevisionNumber;
    public readonly uint Flags;
    public readonly uint PublicKey;
    public readonly uint Name;
    public readonly uint Locale;

    public RawAssemblyRow(uint HashAlgId, ushort MajorVersion, ushort MinorVersion, ushort BuildNumber,
        ushort RevisionNumber, uint Flags, uint PublicKey, uint Name, uint Locale)
    {
        this.HashAlgId = HashAlgId;
        this.MajorVersion = MajorVersion;
        this.MinorVersion = MinorVersion;
        this.BuildNumber = BuildNumber;
        this.RevisionNumber = RevisionNumber;
        this.Flags = Flags;
        this.PublicKey = PublicKey;
        this.Name = Name;
        this.Locale = Locale;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => HashAlgId,
            1 => MajorVersion,
            2 => MinorVersion,
            3 => BuildNumber,
            4 => RevisionNumber,
            5 => Flags,
            6 => PublicKey,
            7 => Name,
            8 => Locale,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed AssemblyProcessor table row
/// </summary>
public class RawAssemblyProcessorRow
{
    public readonly uint Processor;

    public RawAssemblyProcessorRow(uint Processor) => this.Processor = Processor;

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Processor,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed AssemblyOS table row
/// </summary>
public class RawAssemblyOSRow
{
    public readonly uint OSPlatformId;
    public readonly uint OSMajorVersion;
    public readonly uint OSMinorVersion;

    public RawAssemblyOSRow(uint OSPlatformId, uint OSMajorVersion, uint OSMinorVersion)
    {
        this.OSPlatformId = OSPlatformId;
        this.OSMajorVersion = OSMajorVersion;
        this.OSMinorVersion = OSMinorVersion;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => OSPlatformId,
            1 => OSMajorVersion,
            2 => OSMinorVersion,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed AssemblyRef table row
/// </summary>
public class RawAssemblyRefRow
{
    public readonly ushort MajorVersion;
    public readonly ushort MinorVersion;
    public readonly ushort BuildNumber;
    public readonly ushort RevisionNumber;
    public readonly uint Flags;
    public readonly uint PublicKeyOrToken;
    public readonly uint Name;
    public readonly uint Locale;
    public readonly uint HashValue;

    public RawAssemblyRefRow(ushort MajorVersion, ushort MinorVersion, ushort BuildNumber, ushort RevisionNumber,
        uint Flags, uint PublicKeyOrToken, uint Name, uint Locale, uint HashValue)
    {
        this.MajorVersion = MajorVersion;
        this.MinorVersion = MinorVersion;
        this.BuildNumber = BuildNumber;
        this.RevisionNumber = RevisionNumber;
        this.Flags = Flags;
        this.PublicKeyOrToken = PublicKeyOrToken;
        this.Name = Name;
        this.Locale = Locale;
        this.HashValue = HashValue;
        SetMetadataHeader();
    }

    public string name;

    void SetMetadataHeader()
    {
        name = MetadataHeader.StringHeapStream.Read(Name);
    }

    public override string ToString()
    {
        return name;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => MajorVersion,
            1 => MinorVersion,
            2 => BuildNumber,
            3 => RevisionNumber,
            4 => Flags,
            5 => PublicKeyOrToken,
            6 => Name,
            7 => Locale,
            8 => HashValue,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed AssemblyRefProcessor table row
/// </summary>
public class RawAssemblyRefProcessorRow
{
    public readonly uint Processor;
    public readonly uint AssemblyRef;

    public RawAssemblyRefProcessorRow(uint Processor, uint AssemblyRef)
    {
        this.Processor = Processor;
        this.AssemblyRef = AssemblyRef;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Processor,
            1 => AssemblyRef,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed AssemblyRefOS table row
/// </summary>
public class RawAssemblyRefOSRow
{
    public readonly uint OSPlatformId;
    public readonly uint OSMajorVersion;
    public readonly uint OSMinorVersion;
    public readonly uint AssemblyRef;

    public RawAssemblyRefOSRow(uint OSPlatformId, uint OSMajorVersion, uint OSMinorVersion, uint AssemblyRef)
    {
        this.OSPlatformId = OSPlatformId;
        this.OSMajorVersion = OSMajorVersion;
        this.OSMinorVersion = OSMinorVersion;
        this.AssemblyRef = AssemblyRef;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => OSPlatformId,
            1 => OSMajorVersion,
            2 => OSMinorVersion,
            3 => AssemblyRef,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed File table row
/// </summary>
public class RawFileRow
{
    public readonly uint Flags;
    public readonly uint Name;
    public readonly uint HashValue;

    public RawFileRow(uint Flags, uint Name, uint HashValue)
    {
        this.Flags = Flags;
        this.Name = Name;
        this.HashValue = HashValue;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Flags,
            1 => Name,
            2 => HashValue,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed ExportedType table row
/// </summary>
public class RawExportedTypeRow
{
    public readonly uint Flags;
    public readonly uint TypeDefId;
    public readonly uint TypeName;
    public readonly uint TypeNamespace;
    public readonly uint Implementation;

    public RawExportedTypeRow(uint Flags, uint TypeDefId, uint TypeName, uint TypeNamespace, uint Implementation)
    {
        this.Flags = Flags;
        this.TypeDefId = TypeDefId;
        this.TypeName = TypeName;
        this.TypeNamespace = TypeNamespace;
        this.Implementation = Implementation;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Flags,
            1 => TypeDefId,
            2 => TypeName,
            3 => TypeNamespace,
            4 => Implementation,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed ManifestResource table row
/// </summary>
public class RawManifestResourceRow
{
    public readonly uint Offset;
    public readonly uint Flags;
    public readonly uint Name;
    public readonly uint Implementation;

    public RawManifestResourceRow(uint Offset, uint Flags, uint Name, uint Implementation)
    {
        this.Offset = Offset;
        this.Flags = Flags;
        this.Name = Name;
        this.Implementation = Implementation;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Offset,
            1 => Flags,
            2 => Name,
            3 => Implementation,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed NestedClass table row
/// </summary>
public class RawNestedClassRow
{
    public readonly uint NestedClass;
    public readonly uint EnclosingClass;

    public RawNestedClassRow(uint NestedClass, uint EnclosingClass)
    {
        this.NestedClass = NestedClass;
        this.EnclosingClass = EnclosingClass;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => NestedClass,
            1 => EnclosingClass,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed GenericParam table row
/// </summary>
public class RawGenericParamRow
{
    public readonly ushort Number;
    public readonly ushort Flags;
    public readonly uint Owner;
    public readonly uint Name;
    public readonly uint Kind;

    public RawGenericParamRow(ushort Number, ushort Flags, uint Owner, uint Name, uint Kind)
    {
        this.Number = Number;
        this.Flags = Flags;
        this.Owner = Owner;
        this.Name = Name;
        this.Kind = Kind;
    }

    public RawGenericParamRow(ushort Number, ushort Flags, uint Owner, uint Name)
    {
        this.Number = Number;
        this.Flags = Flags;
        this.Owner = Owner;
        this.Name = Name;
        Kind = 0;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Number,
            1 => Flags,
            2 => Owner,
            3 => Name,
            4 => Kind,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed MethodSpec table row
/// </summary>
public class RawMethodSpecRow
{
    public readonly uint Method;
    public readonly uint Instantiation;

    public RawMethodSpecRow(uint Method, uint Instantiation)
    {
        this.Method = Method;
        this.Instantiation = Instantiation;
        SetMetadataHeader();
    }

    void SetMetadataHeader()
    {
        // name = MetadataHeader.StringHeapStream.Read(Name);
    }

    public override string ToString()
    {
        return MetadataHeader.tableStream.ResolveMethodDefOrRef(Method).ToString();
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Method,
            1 => Instantiation,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed GenericParamConstraint table row
/// </summary>
public class RawGenericParamConstraintRow
{
    public readonly uint Owner;
    public readonly uint Constraint;

    public RawGenericParamConstraintRow(uint Owner, uint Constraint)
    {
        this.Owner = Owner;
        this.Constraint = Constraint;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Owner,
            1 => Constraint,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed Document table row
/// </summary>
public class RawDocumentRow
{
    public readonly uint Name;
    public readonly uint HashAlgorithm;
    public readonly uint Hash;
    public readonly uint Language;

    public RawDocumentRow(uint Name, uint HashAlgorithm, uint Hash, uint Language)
    {
        this.Name = Name;
        this.HashAlgorithm = HashAlgorithm;
        this.Hash = Hash;
        this.Language = Language;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Name,
            1 => HashAlgorithm,
            2 => Hash,
            3 => Language,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed MethodDebugInformation table row
/// </summary>
public class RawMethodDebugInformationRow
{
    public readonly uint Document;
    public readonly uint SequencePoints;

    public RawMethodDebugInformationRow(uint Document, uint SequencePoints)
    {
        this.Document = Document;
        this.SequencePoints = SequencePoints;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Document,
            1 => SequencePoints,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed LocalScope table row
/// </summary>
public class RawLocalScopeRow
{
    public readonly uint Method;
    public readonly uint ImportScope;
    public readonly uint VariableList;
    public readonly uint ConstantList;
    public readonly uint StartOffset;
    public readonly uint Length;

    public RawLocalScopeRow(uint Method, uint ImportScope, uint VariableList, uint ConstantList, uint StartOffset,
        uint Length)
    {
        this.Method = Method;
        this.ImportScope = ImportScope;
        this.VariableList = VariableList;
        this.ConstantList = ConstantList;
        this.StartOffset = StartOffset;
        this.Length = Length;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Method,
            1 => ImportScope,
            2 => VariableList,
            3 => ConstantList,
            4 => StartOffset,
            5 => Length,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed LocalVariable table row
/// </summary>
public class RawLocalVariableRow
{
    public readonly ushort Attributes;
    public readonly ushort Index;
    public readonly uint Name;

    public RawLocalVariableRow(ushort Attributes, ushort Index, uint Name)
    {
        this.Attributes = Attributes;
        this.Index = Index;
        this.Name = Name;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Attributes,
            1 => Index,
            2 => Name,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed LocalConstant table row
/// </summary>
public class RawLocalConstantRow
{
    public readonly uint Name;
    public readonly uint Signature;

    public RawLocalConstantRow(uint Name, uint Signature)
    {
        this.Name = Name;
        this.Signature = Signature;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Name,
            1 => Signature,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed ImportScope table row
/// </summary>
public class RawImportScopeRow
{
    public readonly uint Parent;
    public readonly uint Imports;

    public RawImportScopeRow(uint Parent, uint Imports)
    {
        this.Parent = Parent;
        this.Imports = Imports;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Parent,
            1 => Imports,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed StateMachineMethod table row
/// </summary>
public class RawStateMachineMethodRow
{
    public readonly uint MoveNextMethod;
    public readonly uint KickoffMethod;

    public RawStateMachineMethodRow(uint MoveNextMethod, uint KickoffMethod)
    {
        this.MoveNextMethod = MoveNextMethod;
        this.KickoffMethod = KickoffMethod;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => MoveNextMethod,
            1 => KickoffMethod,
            _ => 0,
        };
}

/// <summary>
/// Raw contents of an uncompressed CustomDebugInformation table row
/// </summary>
public class RawCustomDebugInformationRow
{
    public readonly uint Parent;
    public readonly uint Kind;
    public readonly uint Value;

    public RawCustomDebugInformationRow(uint Parent, uint Kind, uint Value)
    {
        this.Parent = Parent;
        this.Kind = Kind;
        this.Value = Value;
    }

    /// <summary>
    /// Gets a column
    /// </summary>
    /// <param name="index">Index of column</param>
    /// <returns></returns>
    public uint this[int index] =>
        index switch
        {
            0 => Parent,
            1 => Kind,
            2 => Value,
            _ => 0,
        };
}