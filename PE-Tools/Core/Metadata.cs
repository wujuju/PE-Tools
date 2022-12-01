using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

public class CLRDirectory
{
    [HeadAttribute("", 4)] public byte[] cb;
    [HeadAttribute("", 2)] public byte[] majorRuntimeVersion;
    [HeadAttribute("", 2)] public byte[] minorRuntimeVersion;
    [ClassAttribute] public DirAttrib metaDir = new DirAttrib();
    [HeadAttribute("", 4)] public byte[] flags;
    [HeadAttribute("", 4)] public byte[] entryPointToken;
    [ClassAttribute] public DirAttrib resourcesDir = new DirAttrib();
    [ClassAttribute] public DirAttrib strongNameSignature = new DirAttrib();
    [ClassAttribute] public DirAttrib codeManagerTable = new DirAttrib();
    [ClassAttribute] public DirAttrib vTableFixups = new DirAttrib();
    [ClassAttribute] public DirAttrib exportAddressTableJumps = new DirAttrib();
    [ClassAttribute] public DirAttrib managedNativeHeader = new DirAttrib();

    public MetadataHeader metadataHeader;
};

public class MetadataHeader
{
    [HeadAttribute("", 4)] public byte[] signature;
    [HeadAttribute("", 2)] public byte[] majorVersion;
    [HeadAttribute("", 2)] public byte[] minorVersion;
    [HeadAttribute("", 4)] public byte[] reserved;
    [HeadAttribute("", 4)] public byte[] stringLength;
    [StringAttribute("stringLength")] public string versionString;
    [HeadAttribute("", 1)] public byte[] versionFirstByte;
    [HeadAttribute("", 1)] public byte[] reserved2;
    [ShortAttribute] public short streamLength;
    public List<HeapStream> streams = new List<HeapStream>();

    public TableStream tableStream;
    public HeapStream StringHeapStream;
    public HeapStream UsHeapStream;
    public HeapStream BlobHeapStream;
    public HeapStream GuidHeapStream;
    public HeapStream PdbHeapStream;
};

public class HeapStream : StreamBase
{
    [IntAttribute] public int offset;
    [IntAttribute] public int streamSize;
    [StringAttribute] public string name;
    public List<HeapStreamString> list = new List<HeapStreamString>();
}

public class HeapStreamString
{
    public int offset;
    public int length;
    public byte[] value;
}


public class TableStreamHeader
{
    [IntAttribute] public int reserved;
    [ByteAttribute] public byte majorVersion;
    [ByteAttribute] public byte minorVersion;
    [ByteAttribute] public byte heapSizes;
    [ByteAttribute] public byte reserved2;
    [LongAttribute] public long valid;
    [LongAttribute] public long sorted;
}

public sealed class ColumnInfo
{
    public readonly byte index;
    public readonly ColumnSize columnSize;
    public readonly string name;
    public uint offset;
    public uint size;

    public ColumnInfo(byte index, string name, ColumnSize columnSize)
    {
        this.index = index;
        this.name = name;
        this.columnSize = columnSize;
    }

    internal uint Unsafe_Read24(ref BytesArray reader)
    {
        Debug.Assert(size == 2 || size == 4);
        return size == 2 ? reader.ReadUInt16() : reader.ReadUInt32();
    }
    
    public uint Read(ref BytesArray reader) =>
        size switch {
            1 => reader.ReadInt8(),
            2 => reader.ReadUInt16(),
            4 => reader.ReadUInt32(),
            _ => throw new InvalidOperationException("Invalid column size"),
        };
}

public sealed class TableInfo
{
    public readonly Table table;
    int rowSize;
    public readonly ColumnInfo[] columns;
    public readonly string name;
    public uint size;
    public uint offset;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="table">Table type</param>
    /// <param name="name">Table name</param>
    /// <param name="columns">All columns</param>
    public TableInfo(Table table, string name, ColumnInfo[] columns)
    {
        this.table = table;
        this.name = name;
        this.columns = columns;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="table">Table type</param>
    /// <param name="name">Table name</param>
    /// <param name="columns">All columns</param>
    /// <param name="rowSize">Row size</param>
    public TableInfo(Table table, string name, ColumnInfo[] columns, int rowSize)
    {
        this.table = table;
        this.name = name;
        this.columns = columns;
        this.rowSize = rowSize;
    }
}

[DebuggerDisplay("R:{numRows} RS:{tableInfo.rowSize} C:{tableInfo.columns.Length} {tableInfo.name}")]
public sealed class MDTable
{
    public readonly Table table;
    public uint numRows;
    public TableInfo tableInfo;

    internal readonly ColumnInfo Column0;
    internal readonly ColumnInfo Column1;
    internal readonly ColumnInfo Column2;
    internal readonly ColumnInfo Column3;
    internal readonly ColumnInfo Column4;
    internal readonly ColumnInfo Column5;
    internal readonly ColumnInfo Column6;
    internal readonly ColumnInfo Column7;
    internal readonly ColumnInfo Column8;
    public bool IsInvalidRID(uint rid) => rid == 0 || rid > numRows;

    internal MDTable(Table table, uint numRows, TableInfo tableInfo)
    {
        this.table = table;
        this.numRows = numRows;
        this.tableInfo = tableInfo;

        var columns = tableInfo.columns;
        int length = columns.Length;
        if (length > 0) Column0 = columns[0];
        if (length > 1) Column1 = columns[1];
        if (length > 2) Column2 = columns[2];
        if (length > 3) Column3 = columns[3];
        if (length > 4) Column4 = columns[4];
        if (length > 5) Column5 = columns[5];
        if (length > 6) Column6 = columns[6];
        if (length > 7) Column7 = columns[7];
        if (length > 8) Column8 = columns[8];
    }

    public void UpdateSize(IList<uint> systemRowCounts, ref uint offset)
    {
        uint colOffset = 0;
        foreach (var colInfo in tableInfo.columns)
        {
            colInfo.offset = colOffset;
            var colSize = GetSize(colInfo.columnSize, systemRowCounts);
            colInfo.size = colSize;
            colOffset += colSize;
        }

        tableInfo.offset = offset;
        tableInfo.size = colOffset * numRows;
        offset += tableInfo.size;
    }

    uint GetSize(ColumnSize columnSize, IList<uint> rowCounts)
    {
        if (ColumnSize.Module <= columnSize && columnSize <= ColumnSize.CustomDebugInformation)
        {
            return 2;
        }
        else if (ColumnSize.TypeDefOrRef <= columnSize && columnSize <= ColumnSize.HasCustomDebugInformation)
        {
            var info = columnSize switch
            {
                ColumnSize.TypeDefOrRef => CodedToken.TypeDefOrRef,
                ColumnSize.HasConstant => CodedToken.HasConstant,
                ColumnSize.HasCustomAttribute => CodedToken.HasCustomAttribute,
                ColumnSize.HasFieldMarshal => CodedToken.HasFieldMarshal,
                ColumnSize.HasDeclSecurity => CodedToken.HasDeclSecurity,
                ColumnSize.MemberRefParent => CodedToken.MemberRefParent,
                ColumnSize.HasSemantic => CodedToken.HasSemantic,
                ColumnSize.MethodDefOrRef => CodedToken.MethodDefOrRef,
                ColumnSize.MemberForwarded => CodedToken.MemberForwarded,
                ColumnSize.Implementation => CodedToken.Implementation,
                ColumnSize.CustomAttributeType => CodedToken.CustomAttributeType,
                ColumnSize.ResolutionScope => CodedToken.ResolutionScope,
                ColumnSize.TypeOrMethodDef => CodedToken.TypeOrMethodDef,
                ColumnSize.HasCustomDebugInformation => CodedToken.HasCustomDebugInformation,
                _ => throw new InvalidOperationException($"Invalid ColumnSize: {columnSize}"),
            };
            uint maxRows = 0;
            foreach (var tableType in info.tableTypes)
            {
                int index = (int)tableType;
                var tableRows = index >= rowCounts.Count ? 0 : rowCounts[index];
                if (tableRows > maxRows)
                    maxRows = tableRows;
            }

            return 2;
        }
        else
        {
            switch (columnSize)
            {
                case ColumnSize.Byte: return 1;
                case ColumnSize.Int16: return 2;
                case ColumnSize.UInt16: return 2;
                case ColumnSize.Int32: return 4;
                case ColumnSize.UInt32: return 4;
                case ColumnSize.Strings: return 2;
                case ColumnSize.GUID: return 2;
                case ColumnSize.Blob: return 2;
            }
        }

        throw new InvalidOperationException($"Invalid ColumnSize: {columnSize}");
    }
}


public sealed class CodedToken
{
    /// <summary>TypeDefOrRef coded token</summary>
    public static readonly CodedToken TypeDefOrRef = new CodedToken(2, new Table[3]
    {
        Table.TypeDef, Table.TypeRef, Table.TypeSpec,
    });

    /// <summary>HasConstant coded token</summary>
    public static readonly CodedToken HasConstant = new CodedToken(2, new Table[3]
    {
        Table.Field, Table.Param, Table.Property,
    });

    /// <summary>HasCustomAttribute coded token</summary>
    public static readonly CodedToken HasCustomAttribute = new CodedToken(5, new Table[24]
    {
        Table.Method, Table.Field, Table.TypeRef, Table.TypeDef,
        Table.Param, Table.InterfaceImpl, Table.MemberRef, Table.Module,
        Table.DeclSecurity, Table.Property, Table.Event, Table.StandAloneSig,
        Table.ModuleRef, Table.TypeSpec, Table.Assembly, Table.AssemblyRef,
        Table.File, Table.ExportedType, Table.ManifestResource, Table.GenericParam,
        Table.GenericParamConstraint, Table.MethodSpec, 0, 0,
    });

    /// <summary>HasFieldMarshal coded token</summary>
    public static readonly CodedToken HasFieldMarshal = new CodedToken(1, new Table[2]
    {
        Table.Field, Table.Param,
    });

    /// <summary>HasDeclSecurity coded token</summary>
    public static readonly CodedToken HasDeclSecurity = new CodedToken(2, new Table[3]
    {
        Table.TypeDef, Table.Method, Table.Assembly,
    });

    /// <summary>MemberRefParent coded token</summary>
    public static readonly CodedToken MemberRefParent = new CodedToken(3, new Table[5]
    {
        Table.TypeDef, Table.TypeRef, Table.ModuleRef, Table.Method,
        Table.TypeSpec,
    });

    /// <summary>HasSemantic coded token</summary>
    public static readonly CodedToken HasSemantic = new CodedToken(1, new Table[2]
    {
        Table.Event, Table.Property,
    });

    /// <summary>MethodDefOrRef coded token</summary>
    public static readonly CodedToken MethodDefOrRef = new CodedToken(1, new Table[2]
    {
        Table.Method, Table.MemberRef,
    });

    /// <summary>MemberForwarded coded token</summary>
    public static readonly CodedToken MemberForwarded = new CodedToken(1, new Table[2]
    {
        Table.Field, Table.Method,
    });

    /// <summary>Implementation coded token</summary>
    public static readonly CodedToken Implementation = new CodedToken(2, new Table[3]
    {
        Table.File, Table.AssemblyRef, Table.ExportedType,
    });

    /// <summary>CustomAttributeType coded token</summary>
    public static readonly CodedToken CustomAttributeType = new CodedToken(3, new Table[5]
    {
        0, 0, Table.Method, Table.MemberRef, 0,
    });

    /// <summary>ResolutionScope coded token</summary>
    public static readonly CodedToken ResolutionScope = new CodedToken(2, new Table[4]
    {
        Table.Module, Table.ModuleRef, Table.AssemblyRef, Table.TypeRef,
    });

    /// <summary>TypeOrMethodDef coded token</summary>
    public static readonly CodedToken TypeOrMethodDef = new CodedToken(1, new Table[2]
    {
        Table.TypeDef, Table.Method,
    });

    /// <summary>HasCustomDebugInformation coded token</summary>
    public static readonly CodedToken HasCustomDebugInformation = new CodedToken(5, new Table[27]
    {
        Table.Method, Table.Field, Table.TypeRef, Table.TypeDef,
        Table.Param, Table.InterfaceImpl, Table.MemberRef, Table.Module,
        Table.DeclSecurity, Table.Property, Table.Event, Table.StandAloneSig,
        Table.ModuleRef, Table.TypeSpec, Table.Assembly, Table.AssemblyRef,
        Table.File, Table.ExportedType, Table.ManifestResource, Table.GenericParam,
        Table.GenericParamConstraint, Table.MethodSpec, Table.Document, Table.LocalScope,
        Table.LocalVariable, Table.LocalConstant, Table.ImportScope,
    });

    public readonly Table[] tableTypes;
    readonly int bits;
    readonly int mask;

    internal CodedToken(int bits, Table[] tableTypes)
    {
        this.bits = bits;
        mask = (1 << bits) - 1;
        this.tableTypes = tableTypes;
    }
}