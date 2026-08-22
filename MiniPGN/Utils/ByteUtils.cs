using System.Text;
using ChessLib.Utils;

namespace MiniPGN.Utils;

public static class ByteUtils
{
    public static byte Extract(this IEnumerator<byte> bytes)
    {
        bytes.MoveNext();
        return bytes.Current;
    }
    
    public static IEnumerable<byte> Extract(this IEnumerator<byte> bytes, int count)
    {
        for (int i = 0; i < count; i++)
            yield return bytes.Extract();
    }
    
    public static byte[] ToByteArray(this string str, bool nullTerminated = false)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);

        if (nullTerminated)
        {
            byte[] nBytes = new byte[bytes.Length + 1];
            Array.Copy(bytes, 0, nBytes, 0, bytes.Length);
            return nBytes;
        }
        
        return bytes;
    }

    public static string GetString(this byte[] bytes)
    {
        return bytes[^1] == 0 ? Encoding.UTF8.GetString(bytes[..^1]) : Encoding.UTF8.GetString(bytes);
    }
    
    public static string ToHexList(this IEnumerable<byte> bytes, bool prefix = false)
    {
        return string.Join(' ', bytes.Select(b => b.ToHex(prefix)));
    }

    public static string ToHex(this byte b, bool prefix = false)
    {
        string hex = Convert.ToString(b, 16).PadLeft(2, '0');
        return prefix ? "0x" + hex : hex;
    }

    public static byte[] ToBytes(this ulong v)
    {
        byte[] bytes = BitConverter.GetBytes(v);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    public static ulong ToUInt64(this byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToUInt64(bytes, 0);
    }
    
    public static byte[] ToBytes(this ushort v)
    {
        byte[] bytes = BitConverter.GetBytes(v);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    public static ulong ToUInt16(this byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToUInt16(bytes, 0);
    }
    
    public static byte[] ToBytes(this uint v)
    {
        byte[] bytes = BitConverter.GetBytes(v);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }
    
    public static ulong ToUInt32(this byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }
    
    public static byte ToByteCoordinate(this int index)
    {
        return ToByteCoordinate(index.AsSquare());
    }
    
    public static byte ToByteCoordinate(this (int file, int rank) square)
    {
        return (byte)(square.rank | (square.file << 3));
    }

    public static byte[] ToByteList(this float v)
    {
        return BitConverter.GetBytes(v);
    }
}