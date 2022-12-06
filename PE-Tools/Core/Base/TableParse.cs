using System;

public partial class TableStream
{
    public string ResolveResolutionScope(uint codedToken)
    {
        if (!CodedToken.ResolutionScope.Decode(codedToken, out uint token))
            return null;
        uint rid = MDToken.ToRID(token);
        if (rid == 0)
            return null;
        return MDToken.ToTable(token) switch
        {
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
            Table.MemberRef => ResolveMemberRef(rid).ToString(),
            Table.MethodSpec => ResolveMethodSpec(rid).ToString(),
            Table.Param => ResolveParam(rid).ToString(),
            Table.InterfaceImpl => ResolveInterfaceImpl(rid).ToString(),
            Table.Constant => ResolveConstant(rid).ToString(),
            Table.DeclSecurity => ResolveDeclSecurity(rid).ToString(),
            Table.ClassLayout => ResolveClassLayout(rid).ToString(),
            Table.StandAloneSig => ResolveStandAloneSig(rid).ToString(),
            Table.Event => ResolveEvent(rid).ToString(),
            Table.Property => ResolveProperty(rid).ToString(),
            Table.TypeSpec => ResolveTypeSpec(rid).ToString(),
            Table.ImplMap => ResolveImplMap(rid).ToString(),
            Table.Assembly => ResolveAssembly(rid).ToString(),
            Table.AssemblyRef => ResolveAssemblyRef(rid).ToString(),
            Table.File => ResolveFile(rid).ToString(),
            Table.ExportedType => ResolveExportedType(rid).ToString(),
            Table.ManifestResource => ResolveManifestResource(rid).ToString(),
            Table.GenericParam => ResolveGenericParam(rid).ToString(),
            Table.GenericParamConstraint => ResolveGenericParamConstraint(rid).ToString(),
            _ => null,
        };
    }

    public RawGenericParamConstraintRow ResolveGenericParamConstraint(uint rid) =>
        listGenericParamConstraintMD[rid - 1];

    public RawGenericParamRow ResolveGenericParam(uint rid) => listGenericParamMD[rid - 1];
    public RawManifestResourceRow ResolveManifestResource(uint rid) => listManifestResourceMD[rid - 1];
    public RawExportedTypeRow ResolveExportedType(uint rid) => listExportedTypeMD[rid - 1];
    public RawFileRow ResolveFile(uint rid) => listFileDefMD[rid - 1];
    public RawAssemblyRow ResolveAssembly(uint rid) => listAssemblyDefMD[rid - 1];
    public RawImplMapRow ResolveImplMap(uint rid) => listImplMapMD[rid - 1];
    public RawTypeSpecRow ResolveTypeSpec(uint rid) => listTypeSpecMD[rid - 1];
    public RawPropertyRow ResolveProperty(uint rid) => listPropertyDefMD[rid - 1];
    public RawEventRow ResolveEvent(uint rid) => listEventDefMD[rid - 1];
    public RawStandAloneSigRow ResolveStandAloneSig(uint rid) => listStandAloneSigMD[rid - 1];
    public RawClassLayoutRow ResolveClassLayout(uint rid) => listClassLayoutMD[rid - 1];
    public RawDeclSecurityRow ResolveDeclSecurity(uint rid) => listDeclSecurityMD[rid - 1];

    public RawInterfaceImplRow ResolveInterfaceImpl(uint rid) => listInterfaceImplMD[rid - 1];

    public RawConstantRow ResolveConstant(uint rid) => listConstantMD[rid - 1];

    public string ResolveMemberRefParent(uint codedToken)
    {
        if (!CodedToken.MemberRefParent.Decode(codedToken, out uint token))
            return null;
        uint rid = MDToken.ToRID(token);
        return MDToken.ToTable(token) switch
        {
            Table.TypeDef => ResolveTypeDef(rid).ToString(),
            Table.TypeRef => ResolveTypeRef(rid).ToString(),
            Table.ModuleRef => ResolveModuleRef(rid).ToString(),
            Table.Method => ResolveMethod(rid).ToString(),
            Table.TypeSpec => ResolveTypeSpec(rid).ToString(),
            _ => null,
        };
    }

    public RawParamRow ResolveParam(uint rid) => listParamDefMD[rid - 1];
    public RawMethodSpecRow ResolveMethodSpec(uint rid) => listMethodSpecMD[rid - 1];
    public RawModuleRow ResolveModule(uint rid) => listModuleDefMD[rid - 1];
    public RawModuleRefRow ResolveModuleRef(uint rid) => listModuleRefMD[rid - 1];

    public RawAssemblyRefRow ResolveAssemblyRef(uint rid) => listAssemblyRefMD[rid - 1];

    public RawTypeRefRow ResolveTypeRef(uint rid) => listTypeRefMD[rid - 1];
    public RawTypeDefRow ResolveTypeDef(uint rid) => listTypeDefMD[rid - 1];
    public RawFieldRow ResolveField(uint rid) => listFieldDefMD[rid - 1];

    public RawMethodRow ResolveMethod(uint rid) => listMethodDefMD[rid - 1];
    public RawMemberRefRow ResolveMemberRef(uint rid) => listMemberRefMD[rid - 1];

    public string ReadTypeSignature(BytesArray reader)
    {
        // ReadType(reader);
        return "";
    }

    object ReadType(BytesArray reader)
    {
        readAgain:
        Il2CppTypeEnum etype = (Il2CppTypeEnum)reader.ReadInt8();
        uint num, i;
        switch (etype)
        {
            case Il2CppTypeEnum.IL2CPP_TYPE_VOID:
                break;
            case Il2CppTypeEnum.IL2CPP_TYPE_BOOLEAN:
            case Il2CppTypeEnum.IL2CPP_TYPE_CHAR:
            case Il2CppTypeEnum.IL2CPP_TYPE_I1:
            case Il2CppTypeEnum.IL2CPP_TYPE_U1:
            case Il2CppTypeEnum.IL2CPP_TYPE_I2:
            case Il2CppTypeEnum.IL2CPP_TYPE_U2:
            case Il2CppTypeEnum.IL2CPP_TYPE_I4:
            case Il2CppTypeEnum.IL2CPP_TYPE_U4:
            case Il2CppTypeEnum.IL2CPP_TYPE_I8:
            case Il2CppTypeEnum.IL2CPP_TYPE_U8:
            case Il2CppTypeEnum.IL2CPP_TYPE_R4:
            case Il2CppTypeEnum.IL2CPP_TYPE_R8:
            {
                // SET_IL2CPPTYPE_VALUE_TYPE(type, 1);
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_STRING:
                break;
            case Il2CppTypeEnum.IL2CPP_TYPE_PTR:
            {
                // Il2CppType* ptrType = (Il2CppType*)IL2CPP_MALLOC(sizeof(Il2CppType));
                // *ptrType = {};
                // ReadType(reader, klassGenericContainer, methodGenericContainer, *ptrType);
                // type.data.type = ptrType;
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_BYREF:
            {
                // type.byref = 1;
                // ReadType(reader, klassGenericContainer, methodGenericContainer, type);
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_VALUETYPE:
            case Il2CppTypeEnum.IL2CPP_TYPE_CLASS:
            {
                var codedToken = (uint)reader.ReadCompressedUint32(out var lengthSize);
                if (CodedToken.TypeDefOrRef.Decode2(codedToken).Table == Table.TypeSpec)
                    return null;
                return ResolveTypeDefOrRef(codedToken);
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_ARRAY:
            {
                // Il2CppArrayType* arrType = (Il2CppArrayType*)IL2CPP_MALLOC_ZERO(sizeof(Il2CppArrayType));
                // ReadArrayType(reader, klassGenericContainer, methodGenericContainer, *arrType);
                // type.data.array = arrType;
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_GENERICINST:
            {
                // var obj = ReadType(reader);
                // num = (uint)reader.ReadCompressedUint32(out var lengthSize);
                // for (int j = 0; j < num; j++)
                // {
                //     ReadType(reader);
                // }
                // Il2CppGenericClass* genericClass = (Il2CppGenericClass*)IL2CPP_MALLOC_ZERO(sizeof(Il2CppGenericClass));
                // ReadGenericClass(reader, klassGenericContainer, methodGenericContainer, *genericClass);
                // type.data.generic_class = genericClass;
                // COPY_IL2CPPTYPE_VALUE_TYPE_FLAG(type, *genericClass->type);
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_TYPEDBYREF:
            {
                // SET_IL2CPPTYPE_VALUE_TYPE(type, 1);
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_I:
            case Il2CppTypeEnum.IL2CPP_TYPE_U:
            {
                // Il2CppTypeEnum.SET_IL2CPPTYPE_VALUE_TYPE(type, 1);
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_FNPTR:
            {
                // RaiseNotSupportedException("Image::ReadType IL2CPP_TYPE_FNPTR");
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_OBJECT:
                break;
            case Il2CppTypeEnum.IL2CPP_TYPE_SZARRAY:
            {
                // Il2CppType* eleType = (Il2CppType*)IL2CPP_MALLOC(sizeof(Il2CppType));
                // *eleType = {};
                // ReadType(reader, klassGenericContainer, methodGenericContainer, *eleType);
                // type.data.type = eleType;
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_VAR:
            {
                // IL2CPP_ASSERT(!klassGenericContainer || !klassGenericContainer->is_method);
                // uint32_t number = reader.ReadCompressedUint32();
                // if (klassGenericContainer)
                // {
                //     //IL2CPP_ASSERT(hybridclr::metadata::IsInterpreterIndex(klassGenericContainer->ownerIndex));
                //     type.data.genericParameterHandle = il2cpp::vm::GlobalMetadata::GetGenericParameterFromIndex((Il2CppMetadataGenericContainerHandle)klassGenericContainer, number);
                // }
                // else
                // {
                //     type.data.__genericParameterIndex = number;
                // }
                /*Il2CppGenericParameter* gp = (Il2CppGenericParameter*)type.data.genericParameterHandle;
                IL2CPP_ASSERT(hybridclr::metadata::IsInterpreterIndex(gp->ownerIndex));*/
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_MVAR:
            {
                // IL2CPP_ASSERT(!methodGenericContainer || methodGenericContainer->is_method);
                // uint32_t number = reader.ReadCompressedUint32();
                // if (methodGenericContainer)
                // {
                //     type.data.genericParameterHandle = il2cpp::vm::GlobalMetadata::GetGenericParameterFromIndex((Il2CppMetadataGenericContainerHandle)methodGenericContainer, number);
                // }
                // else
                // {
                //     // method ref can't resolve at that time
                //     type.data.__genericParameterIndex = number;
                // }
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_CMOD_REQD:
            {
                // ++type.num_mods;
                // uint32_t encodeToken = reader.ReadCompressedUint32();
                // Il2CppType modType = {};
                // ReadTypeFromToken(nullptr, nullptr, DecodeTypeDefOrRefOrSpecCodedIndexTableType(encodeToken), DecodeTypeDefOrRefOrSpecCodedIndexRowIndex(encodeToken), modType);
                // IL2CPP_ASSERT(modType.data.typeHandle);
                // const Il2CppTypeDefinition* modTypeDef = (const Il2CppTypeDefinition*)modType.data.typeHandle;
                // const char* modTypeName = il2cpp::vm::GlobalMetadata::GetStringFromIndex(modTypeDef->nameIndex);
                // const char* modTypeNamespace = il2cpp::vm::GlobalMetadata::GetStringFromIndex(modTypeDef->namespaceIndex);
                // if (std::strcmp(modTypeNamespace, "System.Runtime.InteropServices") == 0)
                // {
                //     if (std::strcmp(modTypeName, "InAttribute") == 0)
                //     {
                //         type.attrs |= PARAM_ATTRIBUTE_IN;
                //     }
                //     else if (std::strcmp(modTypeName, "OutAttribute") == 0)
                //     {
                //         type.attrs |= PARAM_ATTRIBUTE_OUT;
                //     }
                //     else if (std::strcmp(modTypeName, "OptionalAttribute") == 0)
                //     {
                //         type.attrs |= PARAM_ATTRIBUTE_OPTIONAL;
                //     }
                // }
                // goto readAgain;
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_CMOD_OPT:
            {
                // ++type.num_mods;
                // uint32_t encodeToken = reader.ReadCompressedUint32();
                // goto readAgain;
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_INTERNAL:
            {
                // RaiseNotSupportedException("Image::ReadType IL2CPP_TYPE_INTERNAL");
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_MODIFIER:
            {
                // RaiseNotSupportedException("Image::ReadType IL2CPP_TYPE_MODIFIER");
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_SENTINEL:
            {
                break;
            }
            case Il2CppTypeEnum.IL2CPP_TYPE_PINNED:
            {
                // type.pinned = true;
                // ReadType(reader, klassGenericContainer, methodGenericContainer, type);
                break;
            }
            default:
            {
                throw new Exception("Image::ReadType invalid type");
                break;
            }
        }

        return null;
    }

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

    public string ResolveMethodDefOrRef(uint codedToken)
    {
        if (!CodedToken.MethodDefOrRef.Decode(codedToken, out uint token))
            return null;
        uint rid = MDToken.ToRID(token);
        return MDToken.ToTable(token) switch
        {
            Table.Method => ResolveMethod(rid).ToString(),
            Table.MemberRef => ResolveMemberRef(rid).ToString(),
            _ => null,
        };
    }
}

public enum Il2CppTypeEnum
{
    IL2CPP_TYPE_END = 0x00, /* End of List */
    IL2CPP_TYPE_VOID = 0x01,
    IL2CPP_TYPE_BOOLEAN = 0x02,
    IL2CPP_TYPE_CHAR = 0x03,
    IL2CPP_TYPE_I1 = 0x04,
    IL2CPP_TYPE_U1 = 0x05,
    IL2CPP_TYPE_I2 = 0x06,
    IL2CPP_TYPE_U2 = 0x07,
    IL2CPP_TYPE_I4 = 0x08,
    IL2CPP_TYPE_U4 = 0x09,
    IL2CPP_TYPE_I8 = 0x0a,
    IL2CPP_TYPE_U8 = 0x0b,
    IL2CPP_TYPE_R4 = 0x0c,
    IL2CPP_TYPE_R8 = 0x0d,
    IL2CPP_TYPE_STRING = 0x0e,
    IL2CPP_TYPE_PTR = 0x0f, /* arg: <type> token */
    IL2CPP_TYPE_BYREF = 0x10, /* arg: <type> token */
    IL2CPP_TYPE_VALUETYPE = 0x11, /* arg: <type> token */
    IL2CPP_TYPE_CLASS = 0x12, /* arg: <type> token */

    IL2CPP_TYPE_VAR =
        0x13, /* Generic parameter in a generic type definition, represented as number (compressed unsigned integer) number */
    IL2CPP_TYPE_ARRAY = 0x14, /* type, rank, boundsCount, bound1, loCount, lo1 */
    IL2CPP_TYPE_GENERICINST = 0x15, /* <type> <type-arg-count> <type-1> \x{2026} <type-n> */
    IL2CPP_TYPE_TYPEDBYREF = 0x16,
    IL2CPP_TYPE_I = 0x18,
    IL2CPP_TYPE_U = 0x19,
    IL2CPP_TYPE_FNPTR = 0x1b, /* arg: full method signature */
    IL2CPP_TYPE_OBJECT = 0x1c,
    IL2CPP_TYPE_SZARRAY = 0x1d, /* 0-based one-dim-array */

    IL2CPP_TYPE_MVAR =
        0x1e, /* Generic parameter in a generic method definition, represented as number (compressed unsigned integer)  */
    IL2CPP_TYPE_CMOD_REQD = 0x1f, /* arg: typedef or typeref token */
    IL2CPP_TYPE_CMOD_OPT = 0x20, /* optional arg: typedef or typref token */
    IL2CPP_TYPE_INTERNAL = 0x21, /* CLR internal type */

    IL2CPP_TYPE_MODIFIER = 0x40, /* Or with the following types */
    IL2CPP_TYPE_SENTINEL = 0x41, /* Sentinel for varargs method signature */
    IL2CPP_TYPE_PINNED = 0x45, /* Local var that points to pinned object */

    // ==={{ hybridclr
    IL2CPP_TYPE_SYSTEM_TYPE = 0x50,
    IL2CPP_TYPE_BOXED_OBJECT = 0x51,
    IL2CPP_TYPE_FIELD = 0x53,
    IL2CPP_TYPE_PROPERTY = 0x54,
    // ===}} hybridclr

    IL2CPP_TYPE_ENUM = 0x55 /* an enumeration */
}