public partial class TableStream
{
    public string ResolveResolutionScope(uint codedToken) {
        if (!CodedToken.ResolutionScope.Decode(codedToken, out uint token))
            return null;
        uint rid = MDToken.ToRID(token);
        if (rid == 0)
            return null;
        return MDToken.ToTable(token) switch {
            Table.Module => ResolveModule(rid).ToString(),
            Table.ModuleRef => ResolveModuleRef(rid).ToString(),
            Table.AssemblyRef => ResolveAssemblyRef(rid).ToString(),
            Table.TypeRef => ResolveTypeRef(rid).ToString(),
            _ => null,
        };
    }
    public object ResolveTypeDefOrRef(uint codedToken)
    {
        if (!CodedToken.TypeDefOrRef.Decode(codedToken, out uint token))
            return null;
        uint rid = MDToken.ToRID(token);
        return MDToken.ToTable(token) switch
        {
            Table.TypeDef => listTypeDefMD[rid - 1],
            Table.TypeRef => listTypeRefMD[rid - 1],
            _ => null,
        };
    }

    public string ResolveToken(uint token)
    {
        uint rid = MDToken.ToRID(token);
        return MDToken.ToTable(token) switch
        {
            Table.TypeRef => ResolveTypeRef(rid).ToString(),
            Table.TypeDef => ResolveTypeDef(rid).ToString(),
            Table.Field => ResolveField(rid).ToString(),
            Table.Method => ResolveMethod(rid).ToString(),
            // Table.Param => ResolveParam(rid),                                         
            // Table.InterfaceImpl => ResolveInterfaceImpl(rid, gpContext),              
            // Table.MemberRef => ResolveMemberRef(rid, gpContext),                      
        };
    }
    
    public string ResolveMemberRefParent(uint codedToken) {
        if (!CodedToken.MemberRefParent.Decode(codedToken, out uint token))
            return null;
        uint rid = MDToken.ToRID(token);
        return MDToken.ToTable(token) switch {
            Table.TypeDef => ResolveTypeDef(rid).ToString(),
            Table.TypeRef => ResolveTypeRef(rid).ToString(),
            Table.ModuleRef => ResolveModuleRef(rid).ToString(),
            Table.Method => ResolveMethod(rid).ToString(),
            _ => null,
        };
    }

    public RawModuleRow ResolveModule(uint rid) => listModuleDefMD[rid - 1];
    public RawModuleRefRow ResolveModuleRef(uint rid) => listModuleRefMD[rid - 1];
    
    public RawAssemblyRefRow ResolveAssemblyRef(uint rid) => listAssemblyRefMD[rid - 1];
    
    public RawTypeRefRow ResolveTypeRef(uint rid) => listTypeRefMD[rid - 1];
    public RawTypeDefRow ResolveTypeDef(uint rid) => listTypeDefMD[rid - 1];
    public RawFieldRow ResolveField(uint rid) => listFieldDefMD[rid - 1];

    public RawMethodRow ResolveMethod(uint rid) => listMethodDefMD[rid - 1];
    public RawMemberRefRow ResolveMemberRef(uint rid) => listMemberRefMD[rid - 1];


    /// <inheritdoc/>
    public virtual RidList GetFieldRidList(uint typeDefRid) => GetRidList(TypeDefTable, typeDefRid, 4, FieldTable);

    /// <inheritdoc/>
    public virtual RidList GetMethodRidList(uint typeDefRid) => GetRidList(TypeDefTable, typeDefRid, 5, MethodTable);

    /// <inheritdoc/>
    public virtual RidList GetParamRidList(uint methodRid) => GetRidList(MethodTable, methodRid, 5, ParamTable);

    /// <inheritdoc/>
    public virtual RidList GetEventRidList(uint eventMapRid) => GetRidList(EventMapTable, eventMapRid, 1, EventTable);

    /// <inheritdoc/>
    public virtual RidList GetPropertyRidList(uint propertyMapRid) =>
        GetRidList(PropertyMapTable, propertyMapRid, 1, PropertyTable);

    /// <inheritdoc/>
    public virtual RidList GetLocalVariableRidList(uint localScopeRid) =>
        GetRidList(LocalScopeTable, localScopeRid, 2, LocalVariableTable);

    /// <inheritdoc/>
    public virtual RidList GetLocalConstantRidList(uint localScopeRid) =>
        GetRidList(LocalScopeTable, localScopeRid, 3, LocalConstantTable);

    /// <summary>
    /// Gets a rid list (eg. field list)
    /// </summary>
    /// <param name="tableSource">Source table, eg. <c>TypeDef</c></param>
    /// <param name="tableSourceRid">Row ID in <paramref name="tableSource"/></param>
    /// <param name="colIndex">Column index in <paramref name="tableSource"/>, eg. 4 for <c>TypeDef.FieldList</c></param>
    /// <param name="tableDest">Destination table, eg. <c>Field</c></param>
    /// <returns>A new <see cref="RidList"/> instance</returns>
    RidList GetRidList(MDTable tableSource, uint tableSourceRid, int colIndex, MDTable tableDest)
    {
        var column = tableSource.tableInfo.columns[colIndex];
        if (!TryReadColumn24(tableSource, tableSourceRid, column, out uint startRid))
            return RidList.Empty;
        bool hasNext = TryReadColumn24(tableSource, tableSourceRid + 1, column, out uint nextListRid);
        uint lastRid = tableDest.numRows + 1;
        if (startRid == 0 || startRid >= lastRid)
            return RidList.Empty;
        uint endRid =
            !hasNext || (nextListRid == 0 && tableSourceRid + 1 == tableSource.numRows && tableDest.numRows == 0xFFFF)
                ? lastRid
                : nextListRid;
        if (endRid < startRid)
            endRid = startRid;
        if (endRid > lastRid)
            endRid = lastRid;
        return RidList.Create(startRid, endRid - startRid);
    }
}