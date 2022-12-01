public class TableStream : StreamBase
{
    public MDTable[] tables;
    public TableInfo[] infos;
    public uint tablesPos;
    
    IColumnReader columnReader;
    IRowReader<RawMethodRow> methodRowReader;
    public IRowReader<RawMethodRow> MethodRowReader {
        get => methodRowReader;
        set => methodRowReader = value;
    }
    
    #pragma warning disable 1591	// XML doc comment
		public MDTable ModuleTable { get; private set; }
		public MDTable TypeRefTable { get; private set; }
		public MDTable TypeDefTable { get; private set; }
		public MDTable FieldPtrTable { get; private set; }
		public MDTable FieldTable { get; private set; }
		public MDTable MethodPtrTable { get; private set; }
		public MDTable MethodTable { get; private set; }
		public MDTable ParamPtrTable { get; private set; }
		public MDTable ParamTable { get; private set; }
		public MDTable InterfaceImplTable { get; private set; }
		public MDTable MemberRefTable { get; private set; }
		public MDTable ConstantTable { get; private set; }
		public MDTable CustomAttributeTable { get; private set; }
		public MDTable FieldMarshalTable { get; private set; }
		public MDTable DeclSecurityTable { get; private set; }
		public MDTable ClassLayoutTable { get; private set; }
		public MDTable FieldLayoutTable { get; private set; }
		public MDTable StandAloneSigTable { get; private set; }
		public MDTable EventMapTable { get; private set; }
		public MDTable EventPtrTable { get; private set; }
		public MDTable EventTable { get; private set; }
		public MDTable PropertyMapTable { get; private set; }
		public MDTable PropertyPtrTable { get; private set; }
		public MDTable PropertyTable { get; private set; }
		public MDTable MethodSemanticsTable { get; private set; }
		public MDTable MethodImplTable { get; private set; }
		public MDTable ModuleRefTable { get; private set; }
		public MDTable TypeSpecTable { get; private set; }
		public MDTable ImplMapTable { get; private set; }
		public MDTable FieldRVATable { get; private set; }
		public MDTable ENCLogTable { get; private set; }
		public MDTable ENCMapTable { get; private set; }
		public MDTable AssemblyTable { get; private set; }
		public MDTable AssemblyProcessorTable { get; private set; }
		public MDTable AssemblyOSTable { get; private set; }
		public MDTable AssemblyRefTable { get; private set; }
		public MDTable AssemblyRefProcessorTable { get; private set; }
		public MDTable AssemblyRefOSTable { get; private set; }
		public MDTable FileTable { get; private set; }
		public MDTable ExportedTypeTable { get; private set; }
		public MDTable ManifestResourceTable { get; private set; }
		public MDTable NestedClassTable { get; private set; }
		public MDTable GenericParamTable { get; private set; }
		public MDTable MethodSpecTable { get; private set; }
		public MDTable GenericParamConstraintTable { get; private set; }
		public MDTable DocumentTable { get; private set; }
		public MDTable MethodDebugInformationTable { get; private set; }
		public MDTable LocalScopeTable { get; private set; }
		public MDTable LocalVariableTable { get; private set; }
		public MDTable LocalConstantTable { get; private set; }
		public MDTable ImportScopeTable { get; private set; }
		public MDTable StateMachineMethodTable { get; private set; }
		public MDTable CustomDebugInformationTable { get; private set; }
#pragma warning restore
    
   public void InitializeTables() {
			ModuleTable = tables[(int)Table.Module];
			TypeRefTable = tables[(int)Table.TypeRef];
			TypeDefTable = tables[(int)Table.TypeDef];
			FieldPtrTable = tables[(int)Table.FieldPtr];
			FieldTable = tables[(int)Table.Field];
			MethodPtrTable = tables[(int)Table.MethodPtr];
			MethodTable = tables[(int)Table.Method];
			ParamPtrTable = tables[(int)Table.ParamPtr];
			ParamTable = tables[(int)Table.Param];
			InterfaceImplTable = tables[(int)Table.InterfaceImpl];
			MemberRefTable = tables[(int)Table.MemberRef];
			ConstantTable = tables[(int)Table.Constant];
			CustomAttributeTable = tables[(int)Table.CustomAttribute];
			FieldMarshalTable = tables[(int)Table.FieldMarshal];
			DeclSecurityTable = tables[(int)Table.DeclSecurity];
			ClassLayoutTable = tables[(int)Table.ClassLayout];
			FieldLayoutTable = tables[(int)Table.FieldLayout];
			StandAloneSigTable = tables[(int)Table.StandAloneSig];
			EventMapTable = tables[(int)Table.EventMap];
			EventPtrTable = tables[(int)Table.EventPtr];
			EventTable = tables[(int)Table.Event];
			PropertyMapTable = tables[(int)Table.PropertyMap];
			PropertyPtrTable = tables[(int)Table.PropertyPtr];
			PropertyTable = tables[(int)Table.Property];
			MethodSemanticsTable = tables[(int)Table.MethodSemantics];
			MethodImplTable = tables[(int)Table.MethodImpl];
			ModuleRefTable = tables[(int)Table.ModuleRef];
			TypeSpecTable = tables[(int)Table.TypeSpec];
			ImplMapTable = tables[(int)Table.ImplMap];
			FieldRVATable = tables[(int)Table.FieldRVA];
			ENCLogTable = tables[(int)Table.ENCLog];
			ENCMapTable = tables[(int)Table.ENCMap];
			AssemblyTable = tables[(int)Table.Assembly];
			AssemblyProcessorTable = tables[(int)Table.AssemblyProcessor];
			AssemblyOSTable = tables[(int)Table.AssemblyOS];
			AssemblyRefTable = tables[(int)Table.AssemblyRef];
			AssemblyRefProcessorTable = tables[(int)Table.AssemblyRefProcessor];
			AssemblyRefOSTable = tables[(int)Table.AssemblyRefOS];
			FileTable = tables[(int)Table.File];
			ExportedTypeTable = tables[(int)Table.ExportedType];
			ManifestResourceTable = tables[(int)Table.ManifestResource];
			NestedClassTable = tables[(int)Table.NestedClass];
			GenericParamTable = tables[(int)Table.GenericParam];
			MethodSpecTable = tables[(int)Table.MethodSpec];
			GenericParamConstraintTable = tables[(int)Table.GenericParamConstraint];
			DocumentTable = tables[(int)Table.Document];
			MethodDebugInformationTable = tables[(int)Table.MethodDebugInformation];
			LocalScopeTable = tables[(int)Table.LocalScope];
			LocalVariableTable = tables[(int)Table.LocalVariable];
			LocalConstantTable = tables[(int)Table.LocalConstant];
			ImportScopeTable = tables[(int)Table.ImportScope];
			StateMachineMethodTable = tables[(int)Table.StateMachineMethod];
			CustomDebugInformationTable = tables[(int)Table.CustomDebugInformation];
		}
    
    public bool TryReadModuleRow(uint rid, out RawModuleRow row)
    {
        var table = ModuleTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawModuleRow(
            reader.ReadUInt16(),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader),
            table.Column3.Unsafe_Read24(ref reader),
            table.Column4.Unsafe_Read24(ref reader));
        return true;
    }


    /// <summary>
    /// Reads a raw <c>TypeRef</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadTypeRefRow(uint rid, out RawTypeRefRow row)
    {
        var table = TypeRefTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawTypeRefRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>TypeDef</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadTypeDefRow(uint rid, out RawTypeDefRow row)
    {
        var table = TypeDefTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawTypeDefRow(
            reader.ReadUInt32(),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader),
            table.Column3.Unsafe_Read24(ref reader),
            table.Column4.Unsafe_Read24(ref reader),
            table.Column5.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>FieldPtr</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadFieldPtrRow(uint rid, out RawFieldPtrRow row)
    {
        var table = FieldPtrTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawFieldPtrRow(table.Column0.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>Field</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadFieldRow(uint rid, out RawFieldRow row)
    {
        var table = FieldTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawFieldRow(
            reader.ReadUInt16(),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>MethodPtr</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadMethodPtrRow(uint rid, out RawMethodPtrRow row)
    {
        var table = MethodPtrTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawMethodPtrRow(table.Column0.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>Method</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadMethodRow(uint rid, out RawMethodRow row)
    {
        var table = MethodTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        var mrr = methodRowReader;
        if (mrr is not null && mrr.TryReadRow(rid, out row))
            return true;

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawMethodRow(
            reader.ReadUInt32(),
            reader.ReadUInt16(),
            reader.ReadUInt16(),
            table.Column3.Unsafe_Read24(ref reader),
            table.Column4.Unsafe_Read24(ref reader),
            table.Column5.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>ParamPtr</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadParamPtrRow(uint rid, out RawParamPtrRow row)
    {
        var table = ParamPtrTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawParamPtrRow(table.Column0.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>Param</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadParamRow(uint rid, out RawParamRow row)
    {
        var table = ParamTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawParamRow(
            reader.ReadUInt16(),
            reader.ReadUInt16(),
            table.Column2.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>InterfaceImpl</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadInterfaceImplRow(uint rid, out RawInterfaceImplRow row)
    {
        var table = InterfaceImplTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawInterfaceImplRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>MemberRef</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadMemberRefRow(uint rid, out RawMemberRefRow row)
    {
        var table = MemberRefTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawMemberRefRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>Constant</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadConstantRow(uint rid, out RawConstantRow row)
    {
        var table = ConstantTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawConstantRow(
            reader.ReadInt8(),
            reader.ReadInt8(),
            table.Column2.Unsafe_Read24(ref reader),
            table.Column3.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>CustomAttribute</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadCustomAttributeRow(uint rid, out RawCustomAttributeRow row)
    {
        var table = CustomAttributeTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawCustomAttributeRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>FieldMarshal</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadFieldMarshalRow(uint rid, out RawFieldMarshalRow row)
    {
        var table = FieldMarshalTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawFieldMarshalRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>DeclSecurity</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadDeclSecurityRow(uint rid, out RawDeclSecurityRow row)
    {
        var table = DeclSecurityTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawDeclSecurityRow(
            (short)reader.ReadUInt16(),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>ClassLayout</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadClassLayoutRow(uint rid, out RawClassLayoutRow row)
    {
        var table = ClassLayoutTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawClassLayoutRow(
            reader.ReadUInt16(),
            reader.ReadUInt32(),
            table.Column2.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>FieldLayout</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadFieldLayoutRow(uint rid, out RawFieldLayoutRow row)
    {
        var table = FieldLayoutTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawFieldLayoutRow(
            reader.ReadUInt32(),
            table.Column1.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>StandAloneSig</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadStandAloneSigRow(uint rid, out RawStandAloneSigRow row)
    {
        var table = StandAloneSigTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawStandAloneSigRow(table.Column0.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>EventMap</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadEventMapRow(uint rid, out RawEventMapRow row)
    {
        var table = EventMapTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawEventMapRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>EventPtr</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadEventPtrRow(uint rid, out RawEventPtrRow row)
    {
        var table = EventPtrTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawEventPtrRow(table.Column0.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>Event</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadEventRow(uint rid, out RawEventRow row)
    {
        var table = EventTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawEventRow(
            reader.ReadUInt16(),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>PropertyMap</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadPropertyMapRow(uint rid, out RawPropertyMapRow row)
    {
        var table = PropertyMapTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawPropertyMapRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>PropertyPtr</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadPropertyPtrRow(uint rid, out RawPropertyPtrRow row)
    {
        var table = PropertyPtrTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawPropertyPtrRow(table.Column0.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>Property</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadPropertyRow(uint rid, out RawPropertyRow row)
    {
        var table = PropertyTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawPropertyRow(
            reader.ReadUInt16(),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>MethodSemantics</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadMethodSemanticsRow(uint rid, out RawMethodSemanticsRow row)
    {
        var table = MethodSemanticsTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawMethodSemanticsRow(
            reader.ReadUInt16(),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>MethodImpl</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadMethodImplRow(uint rid, out RawMethodImplRow row)
    {
        var table = MethodImplTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawMethodImplRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>ModuleRef</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadModuleRefRow(uint rid, out RawModuleRefRow row)
    {
        var table = ModuleRefTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawModuleRefRow(table.Column0.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>TypeSpec</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadTypeSpecRow(uint rid, out RawTypeSpecRow row)
    {
        var table = TypeSpecTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawTypeSpecRow(table.Column0.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>ImplMap</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadImplMapRow(uint rid, out RawImplMapRow row)
    {
        var table = ImplMapTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawImplMapRow(
            reader.ReadUInt16(),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader),
            table.Column3.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>FieldRVA</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadFieldRVARow(uint rid, out RawFieldRVARow row)
    {
        var table = FieldRVATable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawFieldRVARow(
            reader.ReadUInt32(),
            table.Column1.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>ENCLog</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadENCLogRow(uint rid, out RawENCLogRow row)
    {
        var table = ENCLogTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawENCLogRow(
            reader.ReadUInt32(),
            reader.ReadUInt32());
        return true;
    }

    /// <summary>
    /// Reads a raw <c>ENCMap</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadENCMapRow(uint rid, out RawENCMapRow row)
    {
        var table = ENCMapTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawENCMapRow(reader.ReadUInt32());
        return true;
    }

    /// <summary>
    /// Reads a raw <c>Assembly</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadAssemblyRow(uint rid, out RawAssemblyRow row)
    {
        var table = AssemblyTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawAssemblyRow(
            reader.ReadUInt32(),
            reader.ReadUInt16(),
            reader.ReadUInt16(),
            reader.ReadUInt16(),
            reader.ReadUInt16(),
            reader.ReadUInt32(),
            table.Column6.Unsafe_Read24(ref reader),
            table.Column7.Unsafe_Read24(ref reader),
            table.Column8.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>AssemblyProcessor</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadAssemblyProcessorRow(uint rid, out RawAssemblyProcessorRow row)
    {
        var table = AssemblyProcessorTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawAssemblyProcessorRow(reader.ReadUInt32());
        return true;
    }

    /// <summary>
    /// Reads a raw <c>AssemblyOS</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadAssemblyOSRow(uint rid, out RawAssemblyOSRow row)
    {
        var table = AssemblyOSTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawAssemblyOSRow(
            reader.ReadUInt32(),
            reader.ReadUInt32(),
            reader.ReadUInt32());
        return true;
    }

    /// <summary>
    /// Reads a raw <c>AssemblyRef</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadAssemblyRefRow(uint rid, out RawAssemblyRefRow row)
    {
        var table = AssemblyRefTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawAssemblyRefRow(
            reader.ReadUInt16(),
            reader.ReadUInt16(),
            reader.ReadUInt16(),
            reader.ReadUInt16(),
            reader.ReadUInt32(),
            table.Column5.Unsafe_Read24(ref reader),
            table.Column6.Unsafe_Read24(ref reader),
            table.Column7.Unsafe_Read24(ref reader),
            table.Column8.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>AssemblyRefProcessor</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadAssemblyRefProcessorRow(uint rid, out RawAssemblyRefProcessorRow row)
    {
        var table = AssemblyRefProcessorTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawAssemblyRefProcessorRow(
            reader.ReadUInt32(),
            table.Column1.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>AssemblyRefOS</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadAssemblyRefOSRow(uint rid, out RawAssemblyRefOSRow row)
    {
        var table = AssemblyRefOSTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawAssemblyRefOSRow(
            reader.ReadUInt32(),
            reader.ReadUInt32(),
            reader.ReadUInt32(),
            table.Column3.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>File</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadFileRow(uint rid, out RawFileRow row)
    {
        var table = FileTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawFileRow(
            reader.ReadUInt32(),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>ExportedType</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadExportedTypeRow(uint rid, out RawExportedTypeRow row)
    {
        var table = ExportedTypeTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawExportedTypeRow(
            reader.ReadUInt32(),
            reader.ReadUInt32(),
            table.Column2.Unsafe_Read24(ref reader),
            table.Column3.Unsafe_Read24(ref reader),
            table.Column4.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>ManifestResource</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadManifestResourceRow(uint rid, out RawManifestResourceRow row)
    {
        var table = ManifestResourceTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawManifestResourceRow(
            reader.ReadUInt32(),
            reader.ReadUInt32(),
            table.Column2.Unsafe_Read24(ref reader),
            table.Column3.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>NestedClass</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadNestedClassRow(uint rid, out RawNestedClassRow row)
    {
        var table = NestedClassTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawNestedClassRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>GenericParam</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadGenericParamRow(uint rid, out RawGenericParamRow row)
    {
        var table = GenericParamTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        if (table.Column4 is null)
        {
            row = new RawGenericParamRow(
                reader.ReadUInt16(),
                reader.ReadUInt16(),
                table.Column2.Unsafe_Read24(ref reader),
                table.Column3.Unsafe_Read24(ref reader));
            return true;
        }
        else
        {
            row = new RawGenericParamRow(
                reader.ReadUInt16(),
                reader.ReadUInt16(),
                table.Column2.Unsafe_Read24(ref reader),
                table.Column3.Unsafe_Read24(ref reader),
                table.Column4.Unsafe_Read24(ref reader));
            return true;
        }
    }

    /// <summary>
    /// Reads a raw <c>MethodSpec</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadMethodSpecRow(uint rid, out RawMethodSpecRow row)
    {
        var table = MethodSpecTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawMethodSpecRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>GenericParamConstraint</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadGenericParamConstraintRow(uint rid, out RawGenericParamConstraintRow row)
    {
        var table = GenericParamConstraintTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawGenericParamConstraintRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>Document</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadDocumentRow(uint rid, out RawDocumentRow row)
    {
        var table = DocumentTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawDocumentRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader),
            table.Column3.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>MethodDebugInformation</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadMethodDebugInformationRow(uint rid, out RawMethodDebugInformationRow row)
    {
        var table = MethodDebugInformationTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawMethodDebugInformationRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>LocalScope</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadLocalScopeRow(uint rid, out RawLocalScopeRow row)
    {
        var table = LocalScopeTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawLocalScopeRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader),
            table.Column3.Unsafe_Read24(ref reader),
            reader.ReadUInt32(),
            reader.ReadUInt32());
        return true;
    }

    /// <summary>
    /// Reads a raw <c>LocalVariable</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadLocalVariableRow(uint rid, out RawLocalVariableRow row)
    {
        var table = LocalVariableTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawLocalVariableRow(
            reader.ReadUInt16(),
            reader.ReadUInt16(),
            table.Column2.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>LocalConstant</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadLocalConstantRow(uint rid, out RawLocalConstantRow row)
    {
        var table = LocalConstantTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawLocalConstantRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>ImportScope</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadImportScopeRow(uint rid, out RawImportScopeRow row)
    {
        var table = ImportScopeTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawImportScopeRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>StateMachineMethod</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadStateMachineMethodRow(uint rid, out RawStateMachineMethodRow row)
    {
        var table = StateMachineMethodTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawStateMachineMethodRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a raw <c>CustomDebugInformation</c> row or returns false if the row doesn't exist
    /// </summary>
    /// <param name="rid">Row ID</param>
    /// <param name="row">Row data</param>
    /// <returns></returns>
    public bool TryReadCustomDebugInformationRow(uint rid, out RawCustomDebugInformationRow row)
    {
        var table = CustomDebugInformationTable;
        if (table.IsInvalidRID(rid))
        {
            row = default;
            return false;
        }

        reader.Position = (rid - 1) * table.tableInfo.size;
        row = new RawCustomDebugInformationRow(
            table.Column0.Unsafe_Read24(ref reader),
            table.Column1.Unsafe_Read24(ref reader),
            table.Column2.Unsafe_Read24(ref reader));
        return true;
    }

    /// <summary>
    /// Reads a column
    /// </summary>
    /// <param name="table">The table</param>
    /// <param name="rid">Row ID</param>
    /// <param name="colIndex">Column index in <paramref name="table"/></param>
    /// <param name="value">Result is put here or 0 if we return <c>false</c></param>
    /// <returns><c>true</c> if we could read the column, <c>false</c> otherwise</returns>
    public bool TryReadColumn(MDTable table, uint rid, int colIndex, out uint value) =>
        TryReadColumn(table, rid, table.tableInfo.columns[colIndex], out value);

    /// <summary>
    /// Reads a column
    /// </summary>
    /// <param name="table">The table</param>
    /// <param name="rid">Row ID</param>
    /// <param name="column">Column</param>
    /// <param name="value">Result is put here or 0 if we return <c>false</c></param>
    /// <returns><c>true</c> if we could read the column, <c>false</c> otherwise</returns>
    public bool TryReadColumn(MDTable table, uint rid, ColumnInfo column, out uint value)
    {
        if (table.IsInvalidRID(rid))
        {
            value = 0;
            return false;
        }

        var cr = columnReader;
        if (cr is not null && cr.ReadColumn(table, rid, column, out value))
            return true;

        reader.Position = (rid - 1) * table.tableInfo.size + (uint)column.offset;
        value = column.Read(ref reader);
        return true;
    }

    internal bool TryReadColumn24(MDTable table, uint rid, int colIndex, out uint value) =>
        TryReadColumn24(table, rid, table.tableInfo.columns[colIndex], out value);

    internal bool TryReadColumn24(MDTable table, uint rid, ColumnInfo column, out uint value)
    {
        if (table.IsInvalidRID(rid))
        {
            value = 0;
            return false;
        }

        var cr = columnReader;
        if (cr is not null && cr.ReadColumn(table, rid, column, out value))
            return true;

        reader.Position = (rid - 1) * table.tableInfo.size + (uint)column.offset;
        value = column.size == 2 ? reader.ReadUInt16() : reader.ReadUInt32();
        return true;
    }

    #region 创建Table列表

    public TableInfo[] CreateTables(byte majorVersion, byte minorVersion, out int maxPresentTables)
    {
        int normalMaxTables = (int)Table.CustomDebugInformation + 1;
        maxPresentTables = (majorVersion == 1 && minorVersion == 0) ? (int)Table.NestedClass + 1 : normalMaxTables;
        var tableInfos = new TableInfo[normalMaxTables];

        tableInfos[(int)Table.Module] = new TableInfo(Table.Module, "Module", new ColumnInfo[]
        {
            new ColumnInfo(0, "Generation", ColumnSize.UInt16),
            new ColumnInfo(1, "Name", ColumnSize.Strings),
            new ColumnInfo(2, "Mvid", ColumnSize.GUID),
            new ColumnInfo(3, "EncId", ColumnSize.GUID),
            new ColumnInfo(4, "EncBaseId", ColumnSize.GUID),
        });
        tableInfos[(int)Table.TypeRef] = new TableInfo(Table.TypeRef, "TypeRef", new ColumnInfo[]
        {
            new ColumnInfo(0, "ResolutionScope", ColumnSize.ResolutionScope),
            new ColumnInfo(1, "Name", ColumnSize.Strings),
            new ColumnInfo(2, "Namespace", ColumnSize.Strings),
        });
        tableInfos[(int)Table.TypeDef] = new TableInfo(Table.TypeDef, "TypeDef", new ColumnInfo[]
        {
            new ColumnInfo(0, "Flags", ColumnSize.UInt32),
            new ColumnInfo(1, "Name", ColumnSize.Strings),
            new ColumnInfo(2, "Namespace", ColumnSize.Strings),
            new ColumnInfo(3, "Extends", ColumnSize.TypeDefOrRef),
            new ColumnInfo(4, "FieldList", ColumnSize.Field),
            new ColumnInfo(5, "MethodList", ColumnSize.Method),
        });
        tableInfos[(int)Table.FieldPtr] = new TableInfo(Table.FieldPtr, "FieldPtr", new ColumnInfo[]
        {
            new ColumnInfo(0, "Field", ColumnSize.Field),
        });
        tableInfos[(int)Table.Field] = new TableInfo(Table.Field, "Field", new ColumnInfo[]
        {
            new ColumnInfo(0, "Flags", ColumnSize.UInt16),
            new ColumnInfo(1, "Name", ColumnSize.Strings),
            new ColumnInfo(2, "Signature", ColumnSize.Blob),
        });
        tableInfos[(int)Table.MethodPtr] = new TableInfo(Table.MethodPtr, "MethodPtr", new ColumnInfo[]
        {
            new ColumnInfo(0, "Method", ColumnSize.Method),
        });
        tableInfos[(int)Table.Method] = new TableInfo(Table.Method, "Method", new ColumnInfo[]
        {
            new ColumnInfo(0, "RVA", ColumnSize.UInt32),
            new ColumnInfo(1, "ImplFlags", ColumnSize.UInt16),
            new ColumnInfo(2, "Flags", ColumnSize.UInt16),
            new ColumnInfo(3, "Name", ColumnSize.Strings),
            new ColumnInfo(4, "Signature", ColumnSize.Blob),
            new ColumnInfo(5, "ParamList", ColumnSize.Param),
        });
        tableInfos[(int)Table.ParamPtr] = new TableInfo(Table.ParamPtr, "ParamPtr", new ColumnInfo[]
        {
            new ColumnInfo(0, "Param", ColumnSize.Param),
        });
        tableInfos[(int)Table.Param] = new TableInfo(Table.Param, "Param", new ColumnInfo[]
        {
            new ColumnInfo(0, "Flags", ColumnSize.UInt16),
            new ColumnInfo(1, "Sequence", ColumnSize.UInt16),
            new ColumnInfo(2, "Name", ColumnSize.Strings),
        });
        tableInfos[(int)Table.InterfaceImpl] = new TableInfo(Table.InterfaceImpl, "InterfaceImpl", new ColumnInfo[]
        {
            new ColumnInfo(0, "Class", ColumnSize.TypeDef),
            new ColumnInfo(1, "Interface", ColumnSize.TypeDefOrRef),
        });
        tableInfos[(int)Table.MemberRef] = new TableInfo(Table.MemberRef, "MemberRef", new ColumnInfo[]
        {
            new ColumnInfo(0, "Class", ColumnSize.MemberRefParent),
            new ColumnInfo(1, "Name", ColumnSize.Strings),
            new ColumnInfo(2, "Signature", ColumnSize.Blob),
        });
        tableInfos[(int)Table.Constant] = new TableInfo(Table.Constant, "Constant", new ColumnInfo[]
        {
            new ColumnInfo(0, "Type", ColumnSize.Byte),
            new ColumnInfo(1, "Padding", ColumnSize.Byte),
            new ColumnInfo(2, "Parent", ColumnSize.HasConstant),
            new ColumnInfo(3, "Value", ColumnSize.Blob),
        });
        tableInfos[(int)Table.CustomAttribute] = new TableInfo(Table.CustomAttribute, "CustomAttribute",
            new ColumnInfo[]
            {
                new ColumnInfo(0, "Parent", ColumnSize.HasCustomAttribute),
                new ColumnInfo(1, "Type", ColumnSize.CustomAttributeType),
                new ColumnInfo(2, "Value", ColumnSize.Blob),
            });
        tableInfos[(int)Table.FieldMarshal] = new TableInfo(Table.FieldMarshal, "FieldMarshal", new ColumnInfo[]
        {
            new ColumnInfo(0, "Parent", ColumnSize.HasFieldMarshal),
            new ColumnInfo(1, "NativeType", ColumnSize.Blob),
        });
        tableInfos[(int)Table.DeclSecurity] = new TableInfo(Table.DeclSecurity, "DeclSecurity", new ColumnInfo[]
        {
            new ColumnInfo(0, "Action", ColumnSize.Int16),
            new ColumnInfo(1, "Parent", ColumnSize.HasDeclSecurity),
            new ColumnInfo(2, "PermissionSet", ColumnSize.Blob),
        });
        tableInfos[(int)Table.ClassLayout] = new TableInfo(Table.ClassLayout, "ClassLayout", new ColumnInfo[]
        {
            new ColumnInfo(0, "PackingSize", ColumnSize.UInt16),
            new ColumnInfo(1, "ClassSize", ColumnSize.UInt32),
            new ColumnInfo(2, "Parent", ColumnSize.TypeDef),
        });
        tableInfos[(int)Table.FieldLayout] = new TableInfo(Table.FieldLayout, "FieldLayout", new ColumnInfo[]
        {
            new ColumnInfo(0, "OffSet", ColumnSize.UInt32),
            new ColumnInfo(1, "Field", ColumnSize.Field),
        });
        tableInfos[(int)Table.StandAloneSig] = new TableInfo(Table.StandAloneSig, "StandAloneSig", new ColumnInfo[]
        {
            new ColumnInfo(0, "Signature", ColumnSize.Blob),
        });
        tableInfos[(int)Table.EventMap] = new TableInfo(Table.EventMap, "EventMap", new ColumnInfo[]
        {
            new ColumnInfo(0, "Parent", ColumnSize.TypeDef),
            new ColumnInfo(1, "EventList", ColumnSize.Event),
        });
        tableInfos[(int)Table.EventPtr] = new TableInfo(Table.EventPtr, "EventPtr", new ColumnInfo[]
        {
            new ColumnInfo(0, "Event", ColumnSize.Event),
        });
        tableInfos[(int)Table.Event] = new TableInfo(Table.Event, "Event", new ColumnInfo[]
        {
            new ColumnInfo(0, "EventFlags", ColumnSize.UInt16),
            new ColumnInfo(1, "Name", ColumnSize.Strings),
            new ColumnInfo(2, "EventType", ColumnSize.TypeDefOrRef),
        });
        tableInfos[(int)Table.PropertyMap] = new TableInfo(Table.PropertyMap, "PropertyMap", new ColumnInfo[]
        {
            new ColumnInfo(0, "Parent", ColumnSize.TypeDef),
            new ColumnInfo(1, "PropertyList", ColumnSize.Property),
        });
        tableInfos[(int)Table.PropertyPtr] = new TableInfo(Table.PropertyPtr, "PropertyPtr", new ColumnInfo[]
        {
            new ColumnInfo(0, "Property", ColumnSize.Property),
        });
        tableInfos[(int)Table.Property] = new TableInfo(Table.Property, "Property", new ColumnInfo[]
        {
            new ColumnInfo(0, "PropFlags", ColumnSize.UInt16),
            new ColumnInfo(1, "Name", ColumnSize.Strings),
            new ColumnInfo(2, "Type", ColumnSize.Blob),
        });
        tableInfos[(int)Table.MethodSemantics] = new TableInfo(Table.MethodSemantics, "MethodSemantics",
            new ColumnInfo[]
            {
                new ColumnInfo(0, "Semantic", ColumnSize.UInt16),
                new ColumnInfo(1, "Method", ColumnSize.Method),
                new ColumnInfo(2, "Association", ColumnSize.HasSemantic),
            });
        tableInfos[(int)Table.MethodImpl] = new TableInfo(Table.MethodImpl, "MethodImpl", new ColumnInfo[]
        {
            new ColumnInfo(0, "Class", ColumnSize.TypeDef),
            new ColumnInfo(1, "MethodBody", ColumnSize.MethodDefOrRef),
            new ColumnInfo(2, "MethodDeclaration", ColumnSize.MethodDefOrRef),
        });
        tableInfos[(int)Table.ModuleRef] = new TableInfo(Table.ModuleRef, "ModuleRef", new ColumnInfo[]
        {
            new ColumnInfo(0, "Name", ColumnSize.Strings),
        });
        tableInfos[(int)Table.TypeSpec] = new TableInfo(Table.TypeSpec, "TypeSpec", new ColumnInfo[]
        {
            new ColumnInfo(0, "Signature", ColumnSize.Blob),
        });
        tableInfos[(int)Table.ImplMap] = new TableInfo(Table.ImplMap, "ImplMap", new ColumnInfo[]
        {
            new ColumnInfo(0, "MappingFlags", ColumnSize.UInt16),
            new ColumnInfo(1, "MemberForwarded", ColumnSize.MemberForwarded),
            new ColumnInfo(2, "ImportName", ColumnSize.Strings),
            new ColumnInfo(3, "ImportScope", ColumnSize.ModuleRef),
        });
        tableInfos[(int)Table.FieldRVA] = new TableInfo(Table.FieldRVA, "FieldRVA", new ColumnInfo[]
        {
            new ColumnInfo(0, "RVA", ColumnSize.UInt32),
            new ColumnInfo(1, "Field", ColumnSize.Field),
        });
        tableInfos[(int)Table.ENCLog] = new TableInfo(Table.ENCLog, "ENCLog", new ColumnInfo[]
        {
            new ColumnInfo(0, "Token", ColumnSize.UInt32),
            new ColumnInfo(1, "FuncCode", ColumnSize.UInt32),
        });
        tableInfos[(int)Table.ENCMap] = new TableInfo(Table.ENCMap, "ENCMap", new ColumnInfo[]
        {
            new ColumnInfo(0, "Token", ColumnSize.UInt32),
        });
        tableInfos[(int)Table.Assembly] = new TableInfo(Table.Assembly, "Assembly", new ColumnInfo[]
        {
            new ColumnInfo(0, "HashAlgId", ColumnSize.UInt32),
            new ColumnInfo(1, "MajorVersion", ColumnSize.UInt16),
            new ColumnInfo(2, "MinorVersion", ColumnSize.UInt16),
            new ColumnInfo(3, "BuildNumber", ColumnSize.UInt16),
            new ColumnInfo(4, "RevisionNumber", ColumnSize.UInt16),
            new ColumnInfo(5, "Flags", ColumnSize.UInt32),
            new ColumnInfo(6, "PublicKey", ColumnSize.Blob),
            new ColumnInfo(7, "Name", ColumnSize.Strings),
            new ColumnInfo(8, "Locale", ColumnSize.Strings),
        });
        tableInfos[(int)Table.AssemblyProcessor] = new TableInfo(Table.AssemblyProcessor, "AssemblyProcessor",
            new ColumnInfo[]
            {
                new ColumnInfo(0, "Processor", ColumnSize.UInt32),
            });
        tableInfos[(int)Table.AssemblyOS] = new TableInfo(Table.AssemblyOS, "AssemblyOS", new ColumnInfo[]
        {
            new ColumnInfo(0, "OSPlatformId", ColumnSize.UInt32),
            new ColumnInfo(1, "OSMajorVersion", ColumnSize.UInt32),
            new ColumnInfo(2, "OSMinorVersion", ColumnSize.UInt32),
        });
        tableInfos[(int)Table.AssemblyRef] = new TableInfo(Table.AssemblyRef, "AssemblyRef", new ColumnInfo[]
        {
            new ColumnInfo(0, "MajorVersion", ColumnSize.UInt16),
            new ColumnInfo(1, "MinorVersion", ColumnSize.UInt16),
            new ColumnInfo(2, "BuildNumber", ColumnSize.UInt16),
            new ColumnInfo(3, "RevisionNumber", ColumnSize.UInt16),
            new ColumnInfo(4, "Flags", ColumnSize.UInt32),
            new ColumnInfo(5, "PublicKeyOrToken", ColumnSize.Blob),
            new ColumnInfo(6, "Name", ColumnSize.Strings),
            new ColumnInfo(7, "Locale", ColumnSize.Strings),
            new ColumnInfo(8, "HashValue", ColumnSize.Blob),
        });
        tableInfos[(int)Table.AssemblyRefProcessor] = new TableInfo(Table.AssemblyRefProcessor, "AssemblyRefProcessor",
            new ColumnInfo[]
            {
                new ColumnInfo(0, "Processor", ColumnSize.UInt32),
                new ColumnInfo(1, "AssemblyRef", ColumnSize.AssemblyRef),
            });
        tableInfos[(int)Table.AssemblyRefOS] = new TableInfo(Table.AssemblyRefOS, "AssemblyRefOS", new ColumnInfo[]
        {
            new ColumnInfo(0, "OSPlatformId", ColumnSize.UInt32),
            new ColumnInfo(1, "OSMajorVersion", ColumnSize.UInt32),
            new ColumnInfo(2, "OSMinorVersion", ColumnSize.UInt32),
            new ColumnInfo(3, "AssemblyRef", ColumnSize.AssemblyRef),
        });
        tableInfos[(int)Table.File] = new TableInfo(Table.File, "File", new ColumnInfo[]
        {
            new ColumnInfo(0, "Flags", ColumnSize.UInt32),
            new ColumnInfo(1, "Name", ColumnSize.Strings),
            new ColumnInfo(2, "HashValue", ColumnSize.Blob),
        });
        tableInfos[(int)Table.ExportedType] = new TableInfo(Table.ExportedType, "ExportedType", new ColumnInfo[]
        {
            new ColumnInfo(0, "Flags", ColumnSize.UInt32),
            new ColumnInfo(1, "TypeDefId", ColumnSize.UInt32),
            new ColumnInfo(2, "TypeName", ColumnSize.Strings),
            new ColumnInfo(3, "TypeNamespace", ColumnSize.Strings),
            new ColumnInfo(4, "Implementation", ColumnSize.Implementation),
        });
        tableInfos[(int)Table.ManifestResource] = new TableInfo(Table.ManifestResource, "ManifestResource",
            new ColumnInfo[]
            {
                new ColumnInfo(0, "Offset", ColumnSize.UInt32),
                new ColumnInfo(1, "Flags", ColumnSize.UInt32),
                new ColumnInfo(2, "Name", ColumnSize.Strings),
                new ColumnInfo(3, "Implementation", ColumnSize.Implementation),
            });
        tableInfos[(int)Table.NestedClass] = new TableInfo(Table.NestedClass, "NestedClass", new ColumnInfo[]
        {
            new ColumnInfo(0, "NestedClass", ColumnSize.TypeDef),
            new ColumnInfo(1, "EnclosingClass", ColumnSize.TypeDef),
        });
        if (majorVersion == 1 && minorVersion == 1)
        {
            tableInfos[(int)Table.GenericParam] = new TableInfo(Table.GenericParam, "GenericParam", new ColumnInfo[]
            {
                new ColumnInfo(0, "Number", ColumnSize.UInt16),
                new ColumnInfo(1, "Flags", ColumnSize.UInt16),
                new ColumnInfo(2, "Owner", ColumnSize.TypeOrMethodDef),
                new ColumnInfo(3, "Name", ColumnSize.Strings),
                new ColumnInfo(4, "Kind", ColumnSize.TypeDefOrRef),
            });
        }
        else
        {
            tableInfos[(int)Table.GenericParam] = new TableInfo(Table.GenericParam, "GenericParam", new ColumnInfo[]
            {
                new ColumnInfo(0, "Number", ColumnSize.UInt16),
                new ColumnInfo(1, "Flags", ColumnSize.UInt16),
                new ColumnInfo(2, "Owner", ColumnSize.TypeOrMethodDef),
                new ColumnInfo(3, "Name", ColumnSize.Strings),
            });
        }

        tableInfos[(int)Table.MethodSpec] = new TableInfo(Table.MethodSpec, "MethodSpec", new ColumnInfo[]
        {
            new ColumnInfo(0, "Method", ColumnSize.MethodDefOrRef),
            new ColumnInfo(1, "Instantiation", ColumnSize.Blob),
        });
        tableInfos[(int)Table.GenericParamConstraint] = new TableInfo(Table.GenericParamConstraint,
            "GenericParamConstraint", new ColumnInfo[]
            {
                new ColumnInfo(0, "Owner", ColumnSize.GenericParam),
                new ColumnInfo(1, "Constraint", ColumnSize.TypeDefOrRef),
            });
        tableInfos[0x2D] = new TableInfo((Table)0x2D, string.Empty, new ColumnInfo[] { });
        tableInfos[0x2E] = new TableInfo((Table)0x2E, string.Empty, new ColumnInfo[] { });
        tableInfos[0x2F] = new TableInfo((Table)0x2F, string.Empty, new ColumnInfo[] { });
        tableInfos[(int)Table.Document] = new TableInfo(Table.Document, "Document", new ColumnInfo[]
        {
            new ColumnInfo(0, "Name", ColumnSize.Blob),
            new ColumnInfo(1, "HashAlgorithm", ColumnSize.GUID),
            new ColumnInfo(2, "Hash", ColumnSize.Blob),
            new ColumnInfo(3, "Language", ColumnSize.GUID),
        });
        tableInfos[(int)Table.MethodDebugInformation] = new TableInfo(Table.MethodDebugInformation,
            "MethodDebugInformation", new ColumnInfo[]
            {
                new ColumnInfo(0, "Document", ColumnSize.Document),
                new ColumnInfo(1, "SequencePoints", ColumnSize.Blob),
            });
        tableInfos[(int)Table.LocalScope] = new TableInfo(Table.LocalScope, "LocalScope", new ColumnInfo[]
        {
            new ColumnInfo(0, "Method", ColumnSize.Method),
            new ColumnInfo(1, "ImportScope", ColumnSize.ImportScope),
            new ColumnInfo(2, "VariableList", ColumnSize.LocalVariable),
            new ColumnInfo(3, "ConstantList", ColumnSize.LocalConstant),
            new ColumnInfo(4, "StartOffset", ColumnSize.UInt32),
            new ColumnInfo(5, "Length", ColumnSize.UInt32),
        });
        tableInfos[(int)Table.LocalVariable] = new TableInfo(Table.LocalVariable, "LocalVariable", new ColumnInfo[]
        {
            new ColumnInfo(0, "Attributes", ColumnSize.UInt16),
            new ColumnInfo(1, "Index", ColumnSize.UInt16),
            new ColumnInfo(2, "Name", ColumnSize.Strings),
        });
        tableInfos[(int)Table.LocalConstant] = new TableInfo(Table.LocalConstant, "LocalConstant", new ColumnInfo[]
        {
            new ColumnInfo(0, "Name", ColumnSize.Strings),
            new ColumnInfo(1, "Signature", ColumnSize.Blob),
        });
        tableInfos[(int)Table.ImportScope] = new TableInfo(Table.ImportScope, "ImportScope", new ColumnInfo[]
        {
            new ColumnInfo(0, "Parent", ColumnSize.ImportScope),
            new ColumnInfo(1, "Imports", ColumnSize.Blob),
        });
        tableInfos[(int)Table.StateMachineMethod] = new TableInfo(Table.StateMachineMethod, "StateMachineMethod",
            new ColumnInfo[]
            {
                new ColumnInfo(0, "MoveNextMethod", ColumnSize.Method),
                new ColumnInfo(1, "KickoffMethod", ColumnSize.Method),
            });
        tableInfos[(int)Table.CustomDebugInformation] = new TableInfo(Table.CustomDebugInformation,
            "CustomDebugInformation", new ColumnInfo[]
            {
                new ColumnInfo(0, "Parent", ColumnSize.HasCustomDebugInformation),
                new ColumnInfo(1, "Kind", ColumnSize.GUID),
                new ColumnInfo(2, "Value", ColumnSize.Blob),
            });
        infos = tableInfos;
        return tableInfos;
    }

    #endregion
}