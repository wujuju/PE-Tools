using System;
using System.Text;

public class StreamBase
{
    public BytesArray reader;

    public byte[] ReadUTF16String(uint offset)
    {
        reader.Position = offset;
        int length = ReadCompressedUint32(out int lengthSize);
        var length2 = (int)(length / 2) * 2;
        var bytes = Loadbyte(length2);
        reader.Position += (uint)(length - length2);
        return bytes;
    }

    public string Read(uint offset)
    {
        reader.Position = offset;
        var data = reader.ReadString();
        return Encoding.UTF8.GetString(data);
    }

    public byte[] TryReadBytesUntil()
    {
        return reader.ReadString();
    }

    public virtual Guid ReadGuid() =>
        new Guid(reader.ReadInt32(), reader.ReadInt16(), reader.ReadInt16(),
            reader.ReadInt8(), reader.ReadInt8(), reader.ReadInt8(), reader.ReadInt8(),
            reader.ReadInt8(), reader.ReadInt8(), reader.ReadInt8(), reader.ReadInt8());

    public bool IsValid()
    {
        return reader.IsValid();
    }

    public byte[] Loadbyte(int size)
    {
        return reader.Loadbyte(size);
    }

    public int ReadCompressedUint32(out int lengthSize)
    {
        int firstByte = reader.ReadInt8();
        if (firstByte < 128)
        {
            lengthSize = 1;
            return firstByte;
        }
        else if (firstByte < 192)
        {
            lengthSize = 2;
            return ((firstByte & 0x3f) << 8) | reader.ReadInt8();
        }
        else if (firstByte < 224)
        {
            lengthSize = 4;
            return ((firstByte & 0x1f) << 24) | (((int)reader.ReadInt8()) << 16) |
                   ((int)reader.ReadInt8() << 8) | (int)reader.ReadInt8();
        }
        else
        {
            throw new Exception("bad metadata data. ReadEncodeLength fail");
        }
    }
}