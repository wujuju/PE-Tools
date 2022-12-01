using System;

public class BytesArray
{
    byte[] mbytes;
    int position;
    readonly int startOffset;
    readonly int endOffset;

    public BytesArray(byte[] bytes)
    {
        this.mbytes = bytes;
    }

    // public BytesArray(byte[] bytes, int length, BytesArray bytesArray)
    // {
    //     this.mbytes = bytes;
    //     this.startOffset = startOffset;
    //     this.endOffset = startOffset + length;
    // }

    public BytesArray(byte[] bytes, int startOffset, int length)
    {
        this.mbytes = bytes;
        this.startOffset = position = startOffset;
        this.endOffset = startOffset + length;
    }

    public byte[] Data()
    {
        return mbytes;
    }

    public uint Position
    {
        get => (uint)position;
        set { position = startOffset + (int)value; }
    }

    public bool IsValid()
    {
        return position >= startOffset && position < endOffset;
    }

    public byte ReadInt8()
    {
        var value = mbytes[position];
        position += 1;
        return value;
    }

    public short ReadInt16()
    {
        var value = BitConverter.ToInt16(mbytes, position);
        position += 2;
        return value;
    }

    public int ReadInt32()
    {
        var value = BitConverter.ToInt32(mbytes, position);
        position += 4;
        return value;
    }

    public long ReadInt64()
    {
        var value = BitConverter.ToInt64(mbytes, position);
        position += 8;
        return value;
    }

    public ushort ReadUInt16()
    {
        var value = BitConverter.ToUInt16(mbytes, position);
        position += 2;
        return value;
    }

    public uint ReadUInt32()
    {
        var value = BitConverter.ToUInt32(mbytes, position);
        position += 4;
        return value;
    }

    public ulong ReadUInt64()
    {
        var value = BitConverter.ToUInt64(mbytes, position);
        position += 8;
        return value;
    }

    public byte[] TryReadBytesUntil(byte value)
    {
        int ReadIndex = 0;
        while (true)
        {
            if (mbytes[position + ReadIndex] == 0)
            {
                if (ReadIndex == 0)
                    break;
                var bytes = Loadbyte(ReadIndex);
                position++;
                return bytes;
            }

            ReadIndex++;
        }
        position++;
        return Array.Empty<byte>();
    }

    public byte[] Loadbyte(int size)
    {
        byte[] Data = new byte[size];
        for (int i = 0; i != size; i++)
        {
            Data[i] = mbytes[position];
            position++;
        }

        return Data;
    }
}