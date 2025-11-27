namespace MiniPGN.Chess.Bitboards;
using Chess;
public static class Masks
{
    public const ulong File = 0x8080808080808080; 
    public const ulong Rank = 0xFF00000000000000;
    
    public static readonly ulong[] KnightMasks = new ulong[64];
    public static readonly ulong[] RookMasks = new ulong[64];
    
    public static void Init()
    {
        for (int s = 0; s < 64; s++)
        {
            (int file, int rank) square = s.GetFileRank();

            Console.WriteLine(36.GetFileRank());
            
            KnightMasks[s] = GetKnightMask(square);
            RookMasks[s] = GetRookMask(square);
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