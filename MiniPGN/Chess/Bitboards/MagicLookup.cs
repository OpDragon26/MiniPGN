namespace MiniPGN.Chess.Bitboards;
using static MagicNumbers;
using static Masks;
using static Pins;

public static class MagicLookup
{
    private static readonly ulong[][] RookMoveBitboards = new ulong[64][];
    private static readonly ulong[][] BishopMoveBitboards = new ulong[64][];

    private static readonly ulong[][] RookPinLines = new ulong[64][];
    private static readonly ulong[][] BishopPinLines = new ulong[64][];
    private static readonly List<PinData>[][] RookPinData = new List<PinData>[64][];
    private static readonly List<PinData>[][] BishopPinData = new List<PinData>[64][];

    public static void Init()
    {
        for (int s = 0; s < 64; s++)
        {
            (int file, int rank) square = s.GetFileRank();
            
            ulong[] RookCombinations = Utils.GenerateBitCombinations(RookMasks[s]).Distinct().ToArray();
            //RookMagics[s] = MagicNumberGenerator.GenerateMulti(RookCombinations, shift: 48);
            
            RookMoveBitboards[s] = new ulong[RookMagics[s].highest];
            RookPinLines[s] = new ulong[RookMagics[s].highest];
            RookPinData[s] = new List<PinData>[RookMagics[s].highest];
            foreach (ulong combination in RookCombinations)
            {
                ulong magicIndex = RookMagics[s].Calculate(combination);
                RookMoveBitboards[s][magicIndex] = Utils.GenerateMovesetBitboard(square, combination, RookPattern);
                RookPinLines[s][magicIndex] = Utils.GeneratePinLineBitboard(square, combination, RookPattern);
                RookPinData[s][magicIndex] = GeneratePinData(square, combination, RookPattern);
            }

            ulong[] BishopCombinations = Utils.GenerateBitCombinations(BishopMasks[s]).Distinct().ToArray();
            //BishopMagics[s] = MagicNumberGenerator.GenerateBestMagic(BishopCombinations, 1000, shift: 46);

            BishopMoveBitboards[s] = new ulong[BishopMagics[s].highest];
            BishopPinLines[s] = new ulong[BishopMagics[s].highest];
            BishopPinData[s] = new List<PinData>[BishopMagics[s].highest];
            foreach (ulong combination in BishopCombinations)
            {
                ulong magicIndex = BishopMagics[s].Calculate(combination);
                BishopMoveBitboards[s][magicIndex] = Utils.GenerateMovesetBitboard(square, combination, BishopPattern);
                BishopPinLines[s][magicIndex] = Utils.GeneratePinLineBitboard(square, combination, BishopPattern);
                BishopPinData[s][magicIndex] = GeneratePinData(square, combination, BishopPattern);
            }
            //Console.WriteLine($"Squares done {s + 1}/64");
        }
        //Console.WriteLine(string.Join(", ", BishopMagics));
    }
    
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
        
        public static ulong RookPinLine(int square, ulong blockers)
        {
            ulong magicIndex = RookMagics[square].Calculate(blockers & RookMasks[square]);
            return RookPinLines[square][magicIndex];
        }

        public static ulong BishopPinLine(int square, ulong blockers)
        {
            ulong magicIndex = BishopMagics[square].Calculate(blockers & BishopMasks[square]);
            return BishopPinLines[square][magicIndex];
        }
        
        public static List<PinData> RookPinPath(int square, ulong blockers)
        {
            ulong magicIndex = RookMagics[square].Calculate(blockers & RookMasks[square]);
            return BishopPinData[square][magicIndex];
        }

        public static List<PinData> BishopPinPath(int square, ulong blockers)
        {
            ulong magicIndex = BishopMagics[square].Calculate(blockers & BishopMasks[square]);
            return BishopPinData[square][magicIndex];
        }
    }
}