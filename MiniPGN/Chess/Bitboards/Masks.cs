namespace MiniPGN.Chess.Bitboards;
using Chess;
public static class Masks
{
    public const ulong File = 0x8080808080808080; 
    public const ulong Rank = 0xFF00000000000000;
    
    public const ulong UpDiagonal = 0x102040810204080;
    public const ulong DownDiagonal = 0x8040201008040201;
    
    public static readonly ulong[] KnightMasks = new ulong[64];
    public static readonly ulong[] RookMasks = new ulong[64];
    public static readonly ulong[] BishopMasks = new ulong[64];
    public static readonly ulong[] KingMasks = new ulong[64];
    
    public static void Init()
    {
        for (int s = 0; s < 64; s++)
        {
            (int file, int rank) square = s.GetFileRank();
            
            KnightMasks[s] = GetKnightMask(square);
            RookMasks[s] = GetRookMask(square);
            BishopMasks[s] = GetBishopMask(square);
            KingMasks[s] = GetKingMask(square);
        }
    }

    private static readonly (int file, int rank)[] KnightOffsets = [
        (1, 2), 
        (2, 1),
        (-1, 2), 
        (-2, 1),
        (1, -2), 
        (2, -1),
        (-1, -2), 
        (-2, -1),
    ];

    private static ulong GetKingMask((int file, int rank) square)
    {
        ulong mask = ulong.MaxValue & ~square.Bitboard();
        
        for (int k = 0; k < 8; k++)
        {
            if (k < square.file - 1 || k > square.file + 1)
                mask &= ~Utils.GetFile(k);
            if (k < square.rank - 1 || k > square.rank + 1)
                mask &= ~Utils.GetRank(k);
        }
        
        return mask;
    }
    
    private static ulong GetBishopMask((int file, int rank) square)
    {
        return Utils.GetUpDiagonal(square.file, square.rank) ^ Utils.GetDownDiagonal(square.file, square.rank);
    }
    
    private static ulong GetKnightMask((int file, int rank) square)
    {
        ulong mask = 0;
        
        foreach ((int file, int rank) offset in KnightOffsets)
        {
            (int file, int rank) target = square.Offset(offset);

            if (target.ValidSquare())
                mask |= target.Bitboard();
        }

        return mask;
    }

    private static ulong GetRookMask((int file, int rank) square)
    {
        return Utils.GetFile(square.file) ^ Utils.GetRank(square.rank);
    }
}