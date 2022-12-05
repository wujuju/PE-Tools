using System;

public class HeadAttribute : Attribute
{
    public readonly string name;
    public readonly string tpyeName;
    public readonly int x32Size;
    public readonly int x64Size;

    public HeadAttribute(string headName, int x32Size = 0, int x64Size = -1, string tpyeName = "")
    {
        this.name = headName;
        this.x32Size = x32Size;
        if (x64Size == -1)
            x64Size = x32Size;
        this.x64Size = x64Size;
        this.tpyeName = tpyeName;
    }
}

public class StringAttribute : Attribute
{
    public readonly string name;

    public StringAttribute(string headName = "")
    {
        this.name = headName;
    }
}

public class ShortAttribute : Attribute
{
}

public class UintAttribute : Attribute
{
}

public class IntAttribute : Attribute
{
}

public class LongAttribute : Attribute
{
}

public class ByteAttribute : Attribute
{
}

public class ClassAttribute : Attribute
{
}