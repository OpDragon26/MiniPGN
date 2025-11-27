namespace MiniPGN.Parsing;
using CU = Chess.Board_Representation.Utils;

public static class Utils
{
    private static readonly char[] Files = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];
    
    public static (int file, int rank) ParseSquare(string square)
    {
        return (Array.IndexOf(Files, square[0]), square[1].ToNum() - 1);
    }

    public static byte GetSquareByte((int file, int rank) square)
    {
        return (byte)(square.rank | (square.file << 3));
    }

    private static byte ToNum(this char c)
    {
        return (byte)(c - '0');
    }
}