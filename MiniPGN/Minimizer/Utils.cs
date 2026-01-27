using System.Text;

namespace MiniPGN.Minimizer;

public static class Utils
{
    public static string ToString(IEnumerable<byte> bytes)
    {
        return string.Join(' ' , bytes.Select(b => Convert.ToString(b, 16)
            .PadLeft(2, '0')
            .ToUpper()));
    }

    public static IEnumerable<byte> ToByteArray(this string str, bool nullTerminated)
    {
        IEnumerable<byte> bytes = Encoding.Latin1.GetBytes(str);
        return nullTerminated ? bytes.Append<byte>(0) : bytes;
    }

    public static short ToInt16(this byte[] bytes)
    {
        return BitConverter.ToInt16(bytes, 0);
    }

    public static byte[] ToByteArray(this short value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        return bytes;
    }

    public static byte[] ToByteArray(this ushort value)
    {
        return [(byte)(value >> 8), (byte)value];
    }

    public static string GetString(this IEnumerable<byte> bytes)
    {
        return Encoding.Latin1.GetString(bytes.ToArray());
    }

    public static char GetChar(this byte b)
    {
        return Encoding.Latin1.GetChars([b])[0];
    }

    public static string GetBinaryString(this byte b)
    {
        return Convert.ToString(b, 2).PadLeft(8, '0');
    }
}