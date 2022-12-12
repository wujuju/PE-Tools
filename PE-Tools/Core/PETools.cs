using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

public class PETools
{
    /// <summary>
    /// 转换byte为字符串
    /// </summary>
    /// <param name="Data">byte[]</param>
    /// <returns>AA BB CC DD</returns>
    public static string GetHexString(byte[] Data)
    {
        if (Data == null)
            return "NULL";
        return BitConverter.ToString(Data.Reverse().ToArray()).Replace("-", "");
    }

    public static int GetInt(byte[] Data)
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
    public static string GetHexString(byte[] Data, string Type)
    {
        if (Type.Trim().ToUpper() == "ASCII") return Encoding.ASCII.GetString(Data);
        if (Type.Trim().ToUpper() == "DEFAULT") return Encoding.Default.GetString(Data);
        if (Type.Trim().ToUpper() == "UNICODE") return Encoding.Unicode.GetString(Data);
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

        return GetInt(Data).ToString();
    }


    public static uint GetUint(byte[] Data)
    {
        if (Data.Length == 2)
            return BitConverter.ToUInt16(Data, 0);
        if (Data.Length == 4)
            return BitConverter.ToUInt32(Data, 0);
        Debug.Assert(false);
        return 0;
    }
    
    public static long GetLong(byte[] Data)
    {
        if (Data.Length == 2)
            return BitConverter.ToUInt16(Data, 0);
        if (Data.Length == 4)
            return BitConverter.ToUInt32(Data, 0);
        if (Data.Length == 4)
            return BitConverter.ToInt64(Data, 0);
        return 0;
    }

    public static string Convert16String(int value)
    {
        return BitConverter.ToString(BitConverter.GetBytes(value).Reverse().ToArray()).Replace("-", "");
    }

    public static string Convert16String(uint value)
    {
        return BitConverter.ToString(BitConverter.GetBytes(value).Reverse().ToArray()).Replace("-", "");
    }

    public static string GetString(uint rid, uint offset, ColumnInfo column,
        ColumnSize type = ColumnSize.None)
    {
        if (column != null)
            type = column.columnSize;
        string str = "";
        var tableStream = MetadataHeader.tableStream;
        switch (type)
        {
            case ColumnSize.TypeFlags:
                return ((TypeAttributes)offset).ToString();
            case ColumnSize.FieldFlags:
                return ((FieldAttributes)offset).ToString();
            case ColumnSize.MethodFlags:
                return ((MethodAttributes)offset).ToString();
            case ColumnSize.Strings:
                return MetadataHeader.StringHeapStream.Read(offset);
            case ColumnSize.UInt16:
            case ColumnSize.UInt32:
            case ColumnSize.GUID:
                return offset.ToString();
            case ColumnSize.Int16:
            case ColumnSize.Blob:
                return offset.ToString("X");
            case ColumnSize.TypeDefOrRef:
                var row = tableStream.ResolveTypeDefOrRef(offset);
                if (row is RawTypeDefRow)
                {
                    var typeDef = row as RawTypeDefRow;
                    return typeDef.name;
                }

                if (row is RawTypeRefRow)
                {
                    var typeRef = row as RawTypeRefRow;
                    return typeRef.name;
                }

                return "";
            case ColumnSize.ResolutionScope:
                return tableStream.ResolveResolutionScope(offset);
            case ColumnSize.MemberRefParent:
                return tableStream.ResolveMemberRefParent(offset);
            case ColumnSize.MemberRef:
                var methodRef = tableStream.ResolveMemberRef(offset);
                if (methodRef != null)
                    return tableStream.ResolveMemberRefParent(methodRef.Class) + methodRef.name;
                break;
            case ColumnSize.FieldList:
                var fieldRidList = tableStream.ResolveTypeDef(rid).fieldRidList;
                if (fieldRidList.Count == 0)
                    return "";
                return fieldRidList[0].name;
            case ColumnSize.MethodList:
                var methodRidList = tableStream.ResolveTypeDef(rid).methodRidList;
                if (methodRidList.Count == 0)
                    return "";
                return methodRidList[0].name;
            case ColumnSize.Method:
                var method = tableStream.ResolveMethod(offset);
                if (method != null)
                    return method.name;
                break;
        }

        return offset.ToString();
    }
}