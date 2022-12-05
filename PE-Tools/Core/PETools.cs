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
    
    public static string Convert16String(int value)
    {
        return BitConverter.ToString(BitConverter.GetBytes(value).Reverse().ToArray()).Replace("-", "");
    }
    
    public static string Convert16String(uint value)
    {
        return BitConverter.ToString(BitConverter.GetBytes(value).Reverse().ToArray()).Replace("-", "");
    }
}