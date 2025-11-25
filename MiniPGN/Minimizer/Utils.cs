using System.Text;

namespace MiniPGN.Minimizer;

public static class Utils
{
    public static void AddString(this List<byte> byteList, string str, bool log = false)
    {
        foreach (char c in str)
        {
            IEnumerable<byte> bytes = Encoding.ASCII.GetBytes(c.ToString());
            if (log)
                Console.WriteLine($"'{c}': {string.Join(' ', ToString(bytes))}");
            
            byteList.AddRange(bytes);
        }
    }
    
    public static string ToString(IEnumerable<byte> bytes)
    {
        return string.Join(' ' , bytes.Select(b => Convert.ToString(b, 16)
            .PadLeft(2, '0')
            .ToUpper()));
    }
}