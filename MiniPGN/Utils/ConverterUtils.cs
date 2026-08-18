using ChessLib.API.Parsing;
using ChessLib.Base;
using ChessLib.Bitboards;
using ChessLib.Bitboards.Utils;
using ChessLib.Utils;

namespace MiniPGN.Utils;

public static class ConverterUtils
{
    public static bool IsSingleMove(Board board, Move move)
    {
        ulong mask = AllPieceMask(board, move.Target);
        return (mask & board.Bitboards.AllColor(board.Turn)).Count() == 1;
    }

    private static ulong AllPieceMask(Board board, int index)
    {
        return ParsingUtils.GetFinderMask(board, Pieces.Knight, index).Mask
            | ParsingUtils.GetFinderMask(board, Pieces.Queen, index).Mask;
    }
    
    public static byte ToByteCoordinate(this int index)
    {
        return ToByteCoordinate(index.AsSquare());
    }
    
    public static byte ToByteCoordinate(this (int file, int rank) square)
    {
        return (byte)(square.rank | (square.file << 3));
    }
}