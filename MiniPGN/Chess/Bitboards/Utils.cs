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
    
    public static IEnumerable<ulong> GenerateBitCombinations(ulong mask)
    {
        for (ulong sub = 0; sub != mask; sub = (sub - mask) & mask)
            yield return sub;
        yield return mask;
    }

    public static ulong GenerateMovesetBitboard((int file, int rank) square, ulong blockers, Masks.Pattern pattern)
    {
        ulong moves = 0;
        
        foreach ((int file, int rank) offset in pattern.Offsets)
        {
            for (int m = 1; m < (pattern.Sliding ? 7 : 2); m++)
            {
                (int  file, int rank) target = square.OffsetBy(offset, m);

                if (!target.ValidSquare())
                    break;
                moves |= target.Bitboard();
                if ((target.Bitboard() & blockers) != 0)
                    break;
            }
        }
        
        return moves;
    }

    public static (int file, int rank) FindFileRankFromBitboard(ulong bitboard)
    {
        for (int file = 0; file < 8; file++)
        for (int rank = 0; rank < 8; rank++)
            if ((file, rank).Bitboard() == bitboard)
                return (file, rank);

        return (-1, -1);
    }
}