using MiniPGN.Chess.Board_Representation;

namespace MiniPGN.Chess.Bitboards;

public static class Pins
{
    public static PinState GetPinState(Board board, int side)
    {
        Dictionary<ulong, ulong> paths = new();
        ulong pieces = 0;
        
        ulong rookPins = MagicLookup.Lookup.RookPinLine(board.kingPositions[side], board.AllPieces(1 - side)) & board.AllPieces();
        ulong bishopPins = MagicLookup.Lookup.BishopPinLine(board.kingPositions[side],  board.AllPieces(1 - side)) & board.AllPieces();

        List<PinData> rookPinSearch = MagicLookup.Lookup.RookPinPath(board.kingPositions[side], rookPins);
        List<PinData> bishopPinSearch = MagicLookup.Lookup.BishopPinPath(board.kingPositions[side], bishopPins);

        foreach (PinData rookPin in rookPinSearch)
        {
            ulong pinPieces = board.bitboards.Get(Pieces.WRook, side) | board.bitboards.Get(Pieces.WQueen, side);
            
            if ((pinPieces & rookPin.Pos) != 0)
            {
                paths.Add(rookPin.Pinned, rookPin.Path);
                pieces |= rookPin.Pinned;
            }
        }

        foreach (PinData bishopPin in bishopPinSearch)
        {
            ulong pinPieces = board.bitboards.Get(Pieces.WBishop, side) | board.bitboards.Get(Pieces.WQueen, side);

            if ((pinPieces & bishopPin.Pos) != 0)
            {
                paths.Add(bishopPin.Pinned, bishopPin.Path);
                pieces |= bishopPin.Pinned;
            }
        }
        
        return new PinState(pieces, paths);
    }
    
    public static List<PinData> GeneratePinData((int file, int rank) pos, ulong pieces, Masks.Pattern pattern)
    {
        List<PinData> result = new();
        
        foreach ((int file, int rank) offset in pattern.Offsets)
        {
            int found = 0;
            (int file, int rank) pinPos = (0,0);
            (int file, int rank) pinnedPos = (0,0);
            ulong path = 0;

            for (int d = 1; d < 8; d++)
            {
                (int file, int rank) target = pos.OffsetBy(offset, d);
                
                if (!target.ValidSquare())
                    break;
                if ((pieces & target.Bitboard()) != 0)
                {
                    if (++found > 2)
                        break;
                    if (found == 1)
                        pinnedPos = target;
                    if (found == 2)
                    {
                        pinPos = target;
                        path |= target.Bitboard();
                    }
                }
                else
                    path |= target.Bitboard();
            }
            
            if (found == 2)
                result.Add(new PinData(pinPos.Bitboard(), pinnedPos.Bitboard(), path));
        }
        
        return result;
    }

    public readonly struct PinState(ulong pieces, Dictionary<ulong, ulong> paths)
    {
        public readonly ulong Pieces = pieces;
        public readonly Dictionary<ulong, ulong> Paths = paths;
    }
    
    public class PinData(ulong pos, ulong pinned, ulong path)
    {
        public readonly ulong Pos = pos;
        public readonly ulong Pinned = pinned;
        public readonly ulong Path = path;
    }
}