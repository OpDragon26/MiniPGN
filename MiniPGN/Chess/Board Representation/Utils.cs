namespace MiniPGN.Chess.Board_Representation;

public static class Utils
{
    public static int GetIndex(int file, int rank)
    {
        return rank * 8 + file;
    }
    
    public static int GetIndex((int file, int rank) square)
    {
        return square.rank * 8 + square.file;
    }

    public static ulong GetSquare(int index)
    {
        return 1UL << index;
    }

    public static ulong GetSquare(int file, int rank)
    {
        return GetSquare(GetIndex(file, rank));
    }
}