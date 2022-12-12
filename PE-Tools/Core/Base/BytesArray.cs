using System;
using System.Diagnostics;
using System.Text;

public class BytesArray
{
    byte[] mbytes;
    int position;
    readonly int startOffset;
    readonly int endOffset;

    public BytesArray(byte[] bytes)
    {
        this.mbytes = bytes;
        this.startOffset = position = 0;
        this.endOffset = bytes.Length;
    }

    public BytesArray(BytesArray bytes, uint startOffset, uint length)
    {
        this.mbytes = bytes.mbytes;
        this.startOffset = bytes.RealPostion + (int)startOffset;
        this.endOffset = (int)this.startOffset + (int)length;
        Debug.Assert(this.startOffset + length <= bytes.endOffset);
    }

    public byte[] Data()
    {
        return mbytes;
    }

    public uint Position
    {
        get => (uint)position;
        set { position = (int)value; }
    }

    public int RealPostion
    {
        get => startOffset + position;
    }

    public bool IsValid()
    {
        return RealPostion >= startOffset && RealPostion < endOffset;
    }

    public byte ReadInt8()
    {
        var value = mbytes[RealPostion];
        position += 1;
        return value;
    }

    public sbyte ReadSInt8()
    {
        var value = (sbyte)mbytes[RealPostion];
        position += 1;
        return value;
    }

    public float ReadSingle()
    {
        var value = BitConverter.ToSingle(mbytes, RealPostion);
        position += 4;
        return value;
    }

    public short ReadInt16()
    {
        var value = BitConverter.ToInt16(mbytes, RealPostion);
        position += 2;
        return value;
    }

    public int ReadInt32()
    {
        var value = BitConverter.ToInt32(mbytes, RealPostion);
        position += 4;
        return value;
    }

    public long ReadInt64()
    {
        var value = BitConverter.ToInt64(mbytes, RealPostion);
        position += 8;
        return value;
    }

    public ushort ReadUInt16()
    {
        var value = BitConverter.ToUInt16(mbytes, RealPostion);
        position += 2;
        return value;
    }

    public uint ReadUInt32()
    {
        var value = BitConverter.ToUInt32(mbytes, RealPostion);
        position += 4;
        return value;
    }

    private byte[] TryReadBytesUntil(byte value)
    {
        int ReadIndex = 0;
        while (true)
        {
            if (mbytes[RealPostion + ReadIndex] == 0)
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

    public byte[] ReadString(int length = 0)
    {
        if (length > 0)
            return Loadbyte(length);
        else
            return TryReadBytesUntil(0);
    }

    public string ReadUTF8String(int length = 0)
    {
        return Encoding.UTF8.GetString(ReadString(length));
    }

    public byte[] Loadbyte(int size)
    {
        byte[] Data = new byte[size];
        for (int i = 0; i != size; i++)
        {
            Data[i] = mbytes[RealPostion];
            position++;
        }

        return Data;
    }

    public void Loadbyte(ref byte[] Data)
    {
        for (int i = 0; i != Data.Length; i++)
        {
            Data[i] = mbytes[RealPostion];
            position++;
        }
    }

    public int ReadCompressedUint32(out int lengthSize)
    {
        int firstByte = ReadInt8();
        if (firstByte < 128)
        {
            lengthSize = 1;
            return firstByte;
        }
        else if (firstByte < 192)
        {
            lengthSize = 2;
            return ((firstByte & 0x3f) << 8) | ReadInt8();
        }
        else if (firstByte < 224)
        {
            lengthSize = 4;
            return ((firstByte & 0x1f) << 24) | (((int)ReadInt8()) << 16) |
                   ((int)ReadInt8() << 8) | (int)ReadInt8();
        }
        else
        {
            throw new Exception("bad metadata data. ReadEncodeLength fail");
        }
    }

    public unsafe byte* GetPtr()
    {
        fixed (byte* ptr = mbytes)
        {
            byte* bytePtr = ptr + RealPostion;
            return bytePtr;
        }
    }
}