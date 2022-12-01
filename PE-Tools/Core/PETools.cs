using System;
using System.Linq;

public class PETools
{
    public static string Convert16String(int value)
    {
        return BitConverter.ToString(BitConverter.GetBytes(value).Reverse().ToArray()).Replace("-", "");
    }
    
    public static string Convert16String(uint value)
    {
        return BitConverter.ToString(BitConverter.GetBytes(value).Reverse().ToArray()).Replace("-", "");
    }
}