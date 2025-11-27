namespace MiniPGN.Chess.Bitboards;
using static MagicNumbers;
using static Masks;

public static class MagicLookup
{
    private static readonly ulong[][] RookMoveBitboards = new ulong[64][];

    public static class Lookup
    {
        public static ulong RookBitboard(int square, ulong blockers)
        {
            return RookMoveBitboards[square][RookMagics[square].Calculate(blockers & Masks.RookMasks[square])];
        }
    }

    public static void Init()
    {
        for (int s = 0; s < 64; s++)
        {
            (int file, int rank) square = s.GetFileRank();
            
            ulong[] RookCombinations = Utils.GenerateBitCombinations(RookMasks[s]).Distinct().ToArray();
            //RookMagics[s] = MagicNumberGenerator.GenerateMulti(RookCombinations, shift: 48);
            
            RookMoveBitboards[s] = new ulong[RookMagics[s].highest];
            foreach (ulong combination in RookCombinations)
            {
                ulong magicIndex = RookMagics[s].Calculate(combination);
                RookMoveBitboards[s][magicIndex] = Utils.GenerateMovesetBitboard(square, combination, RookPattern);
            }

            //Console.WriteLine($"Squares done {s + 1}/64");
        }
        
        //Console.WriteLine(string.Join(", ", RookMagics));

    }
}