using ChessLib.API.Parsing;
using ChessLib.Base;
using ChessLib.Base.Utils;
using ChessLib.Bitboards;
using ChessLib.Bitboards.Utils;
using ChessLib.Utils;

namespace MiniPGN.Utils;

public static class ConverterUtils
{
    public static bool IsSingleMove(Board board, Move move)
    {
        ulong knights = GetMask(board, Pieces.Knight, move.Target) & GetPiece(board, Pieces.Knight);
        ulong bishop = GetMask(board, Pieces.Bishop, move.Target) & (GetPiece(board, Pieces.Bishop) | GetPiece(board, Pieces.Queen));
        ulong rook = GetMask(board, Pieces.Rook, move.Target) & (GetPiece(board, Pieces.Rook) | GetPiece(board, Pieces.Queen));
        ulong king = GetMask(board, Pieces.King, move.Target) & GetPiece(board, Pieces.King);

        return (knights | bishop | rook | king).Count() == 1;
    }

    static ulong GetPiece(Board board, byte type)
    {
        return board.Bitboards[type.AsColor(board.Turn)];
    }

    static ulong GetMask(Board board, byte type, int square)
    {
        return ParsingUtils.GetFinderMask(board, type, square).Mask;
    }
}