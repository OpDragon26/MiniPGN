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
        IEnumerable<byte> bytes = Encoding.UTF8.GetBytes(str);
        return nullTerminated ? bytes.Append<byte>(0) : bytes;
    }

    public static byte[] ToByteArray(this short value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    public static byte[] ToByteArray(this ushort value)
    {
        return [(byte)(value >> 8), (byte)value];
    }
}