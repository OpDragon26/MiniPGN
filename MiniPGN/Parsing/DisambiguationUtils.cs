using MiniPGN.Chess;
using MiniPGN.Chess.Board_Representation;
using MiniPGN.Minimizer;
using static MiniPGN.Chess.Board_Representation.Pieces;
using MiniPGN.Chess.Bitboards;

namespace MiniPGN.Parsing;

public static class DisambiguationUtils
{
    public static MovingPiece FindMovingPiece(Board board, (int file, int rank) target, byte piece, Disambiguation d = Disambiguation.None, int dNum = 0)
    {
        if (d == Disambiguation.None)
        {
            ulong allPieces = FindMovingPieceBitboard(board, target);
            if (ulong.PopCount(allPieces) == 1)
                return new MovingPiece(Chess.Bitboards.Utils.FindFileRankFromBitboard(allPieces), piece, true);   
        }

        ulong specifiedPieces = FindMovingPieceBitboard(board, target, piece);
        
        if (d == Disambiguation.File)
            specifiedPieces &= Chess.Bitboards.Utils.GetFile(dNum);
        else if (d == Disambiguation.Rank)
            specifiedPieces &= Chess.Bitboards.Utils.GetRank(dNum);
        
        if (ulong.PopCount(specifiedPieces) == 1)
            return new MovingPiece(Chess.Bitboards.Utils.FindFileRankFromBitboard(specifiedPieces), piece, false);

        if (ulong.PopCount(specifiedPieces) == 0)
            throw new NotationParsingException("Could not find moving piece");
        throw new NotationParsingException("Insufficient disambiguation");
    }

    private static ulong FindMovingPieceBitboard(Board board, (int file, int rank) target, byte piece)
    {
        int targetIndex = target.GetIndex();
        
        return TypeOf(piece) switch
        {
            WKnight => FindKnightBitboard(board, targetIndex),
            WBishop => FindBishopBitboard(board, targetIndex),
            WRook => FindRookBitboard(board, targetIndex),
            WQueen => FindQueenBitboard(board, targetIndex),
            WKing => FindKingBitboard(board, targetIndex),
            _ => throw new NotationParsingException($"Could not find moving bitboard of piece {piece}")
        };
    }

    private static ulong FindMovingPieceBitboard(Board board, (int file, int rank) target)
    {
        int targetIndex = target.GetIndex();

        return FindKnightBitboard(board, targetIndex)
               | FindBishopBitboard(board, targetIndex)
               | FindRookBitboard(board, targetIndex)
               | FindQueenBitboard(board, targetIndex)
               | FindKingBitboard(board, targetIndex);
    }

    private static (ulong source, byte piece) FindFreePieceTypeByBitboard(Board board, (int file, int rank) target)
    {
        int targetIndex = target.GetIndex();

        ulong knight = FindKnightBitboard(board, targetIndex);
        if (knight != 0)
            return (knight, WKnight);
        ulong bishop = FindBishopBitboard(board, targetIndex);
        if (bishop != 0)
            return (bishop, WBishop);
        ulong rook = FindRookBitboard(board, targetIndex);
        if (rook != 0)
            return (rook, WRook);
        ulong queen = FindQueenBitboard(board, targetIndex);
        if (queen != 0)
            return (queen, WQueen);
        ulong king = FindKingBitboard(board, targetIndex);
        if (king != 0)
            return (king, WKing);

        throw new NotationParsingException($"No piece could move to {target}");
    }
    
    public static MovingPiece FindFreePiece(Board board, (int file, int rank) target)
    {
        (ulong square, byte piece) = FindFreePieceTypeByBitboard(board, target);

        (int file, int rank) source = Chess.Bitboards.Utils.FindFileRankFromBitboard(square);

        return new(source, piece, true);
    }
    
    public struct MovingPiece((int file, int rank) source, byte piece, bool freePiece)
    {
        public readonly (int file, int rank) Source = source;
        public readonly byte Piece = piece;
        public readonly bool FreePiece = freePiece;
    }
    
    public enum Disambiguation
    {
        None,
        File,
        Rank,
        Double
    }
    
    private static ulong FindKnightBitboard(Board board, int targetIndex)
    {
        return Masks.KnightMasks[targetIndex] & board.GetBitboard(board.turn, WKnight);
    }

    private static ulong FindBishopBitboard(Board board, int targetIndex)
    {
        return MagicLookup.Lookup.BishopBitboard(targetIndex, 
            board.AllPieces()) & board.GetBitboard(board.turn, WBishop);
    }

    private static ulong FindRookBitboard(Board board, int targetIndex)
    {
       return MagicLookup.Lookup.RookBitboard(targetIndex,
            board.AllPieces()) & board.GetBitboard(board.turn, WRook);
    }

    private static ulong FindQueenBitboard(Board board, int targetIndex)
    {
        return MagicLookup.Lookup.QueenBitboards(targetIndex,
            board.AllPieces()) & board.GetBitboard(board.turn, WQueen);
    }

    private static ulong FindKingBitboard(Board board, int targetIndex)
    {
        return Masks.KingMasks[targetIndex] & board.GetBitboard(board.turn, WKing);
    }
}