namespace MiniPGN.Chess.Bitboards;

public static class Utils
{
    public static ulong GetFile(int file)
    {
        return Masks.File >> (7 - file);
    }
    
    public static ulong GetRank(int rank)
    {
        return Masks.Rank >> (8 * (7 - rank));
    }

    public static ulong GetUpDiagonal(int file, int rank)
    {
        int push = 7 - rank - file;
        return push >= 0 ? Masks.UpDiagonal >> push * 8 : Masks.UpDiagonal << -push * 8;
    }

    public static ulong GetDownDiagonal(int file, int rank)
    {
        int push = file - rank;
        return push >= 0 ? Masks.DownDiagonal >> push * 8 : Masks.DownDiagonal << -push * 8;
    }
}