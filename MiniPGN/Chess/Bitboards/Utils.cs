namespace MiniPGN.Chess.Bitboards;

public static class Utils
{
    public static ulong GetFile(int file)
    {
        return Masks.File >> (7 - file);
    }
    
    public static ulong GetRank(int rank)
    {
        return Masks.Rank >> (8 * rank);
    }
}