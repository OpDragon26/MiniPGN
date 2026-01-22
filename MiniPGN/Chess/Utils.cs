namespace MiniPGN.Chess;

public static class Utils
{
    private static readonly Random Random = new();
    private static readonly char[] Files = ['a','b','c','d','e','f','g','h'];
    private static readonly char[] Ranks = ['1','2','3','4','5','6','7','8'];
    
    public static ulong RandomUlong()
    {
        return (ulong)Random.Next() * (ulong)Random.Next();
    }
    
    public static int GetIndex(int file, int rank)
    {
        return rank * 8 + file;
    }
    
    public static int GetIndex(this (int file, int rank) square)
    {
        return square.rank * 8 + square.file;
    }

    public static ulong GetSquare(int index)
    {
        return 1UL << index;
    }

    public static (int file, int rank) GetFileRank(this int index)
    {
        return (index % 8, index / 8);
    }
    
    public static ulong GetSquare(int file, int rank)
    {
        return GetSquare(GetIndex(file, rank));
    }

    public static ulong Bitboard(this (int file, int rank) square)
    {
        return GetSquare(square.file, square.rank);
    }
    
    public static bool OnSameRank(int square1, int square2)
    {
        return square1 / 8 == square2 / 8;
    }

    public static bool OnSameFile(int square1, int square2)
    {
        return square1 % 8 == square2 % 8;
    }

    public static bool ValidSquare(int file, int rank)
    {
        return (file is >= 0 and < 8) && (rank is >= 0 and < 8);
    }

    public static bool ValidSquare(this (int file, int rank) square)
    {
        return ValidSquare(square.file, square.rank);
    }

    public static (int file, int rank) OffsetBy(this (int file, int rank) square, (int file, int rank) other, int multiplier = 1)
    {
        return (square.file + other.file * multiplier, square.rank + other.rank * multiplier);
    }

    public static string SquareString(this (int file, int rank) square)
    {
        char f = Files[square.file];
        char r = Ranks[square.rank];

        return $"{f}{r}";
    }
}