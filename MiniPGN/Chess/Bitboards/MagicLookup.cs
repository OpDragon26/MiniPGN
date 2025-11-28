namespace MiniPGN.Chess.Bitboards;
using static MagicNumbers;
using static Masks;

public static class MagicLookup
{
    private static readonly ulong[][] RookMoveBitboards = new ulong[64][];
    private static readonly ulong[][] BishopMoveBitboards = new ulong[64][];

    public static class Lookup
    {
        public static ulong RookBitboard(int square, ulong blockers)
        {
            ulong magicIndex = RookMagics[square].Calculate(blockers & RookMasks[square]);
            return RookMoveBitboards[square][magicIndex];
        }

        public static ulong BishopBitboard(int square, ulong blockers)
        {
            ulong magicIndex = BishopMagics[square].Calculate(blockers & BishopMasks[square]);
            return BishopMoveBitboards[square][magicIndex];
        }

        public static ulong QueenBitboards(int square, ulong blockers)
        {
            return RookBitboard(square, blockers) | BishopBitboard(square, blockers);
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

            ulong[] BishopCombinations = Utils.GenerateBitCombinations(BishopMasks[s]).Distinct().ToArray();
            //BishopMagics[s] = MagicNumberGenerator.GenerateBestMagic(BishopCombinations, 1000, shift: 46);

            BishopMoveBitboards[s] = new ulong[BishopMagics[s].highest];
            foreach (ulong combination in BishopCombinations)
            {
                ulong magicIndex = BishopMagics[s].Calculate(combination);
                BishopMoveBitboards[s][magicIndex] = Utils.GenerateMovesetBitboard(square, combination, BishopPattern);
            }

            //Console.WriteLine($"Squares done {s + 1}/64");
        }
        
        //Console.WriteLine(string.Join(", ", BishopMagics));

    }
}