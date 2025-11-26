namespace MiniPGN.Parsing;
using CU = Chess.Board_Representation.Utils;

public static class Utils
{
    private static readonly char[] Files = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];
    
    public static (int file, int rank) ParseSquare(string square)
    {
        return (Array.IndexOf(Files, square[0]), (byte)square[1] - 30);
    }

    public static byte GetSquareByte((int file, int rank) square)
    {
        return (byte)(square.file | (square.rank << 3));
    }
}