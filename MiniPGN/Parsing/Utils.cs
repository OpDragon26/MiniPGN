namespace MiniPGN.Parsing;

public static class Utils
{
    private static readonly char[] Files = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];
    
    public static (int file, int rank) ParseSquare(string square)
    {
        return (AsFile(square[0]), square[1].ToNum() - 1);
    }

    public static byte ToByte(this (int file, int rank) square)
    {
        return (byte)(square.rank | (square.file << 3));
    }

    public static byte ToNum(this char c)
    {
        return (byte)(c - '0');
    }

    public static int AsFile(this char f)
    {
        return Array.IndexOf(Files, f);
    }
    
    public static bool IsFile(this char f)
    {
        return Files.Contains(f);
    }

    public static char ToFile(this byte f)
    {
        return Files[f];
    }

    public static (int file, int rank) AsSquare(this byte b)
    {
        int file = (b >> 3) & 0b111;
        int rank = b & 0b111;
        return (file, rank);
    }
}