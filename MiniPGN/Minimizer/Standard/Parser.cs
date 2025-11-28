using MiniPGN.Chess;
using MiniPGN.Chess.Bitboards;
using MiniPGN.Chess.Parsing;

namespace MiniPGN.Minimizer.Standard;
using Chess.Board_Representation;
using Parsing;
using static Chess.Board_Representation.Pieces;

public class Parser : GameParser
{
    private static readonly MoveResult WhiteShortCastle = new([0b111_00_110, 0b110_000_00], new( 4,  6, flag: Flag.WhiteShortCastle));
    private static readonly MoveResult BlackShortCastle = new([0b111_00_110, 0b110_111_00], new(60, 62, flag: Flag.BlackShortCastle));
    private static readonly MoveResult WhiteLongCastle = new([0b111_00_110, 0b010_000_00], new( 4,  6, flag: Flag.WhiteLongCastle));
    private static readonly MoveResult BlackLongCastle = new([0b111_00_110, 0b010_111_00], new(60, 62, flag: Flag.BlackLongCastle));
    
    protected override MoveResult ParseMove(string alg, Board board)
    {
        if (alg.EndsWith('#') || alg.EndsWith('+'))
            alg = alg[..^1];

        if (alg.Equals("O-O"))
            return board.turn == 0 ? WhiteShortCastle : BlackShortCastle;
        if (alg.Equals("O-O-O"))
            return board.turn == 0 ? WhiteLongCastle : BlackLongCastle;
        
        switch (alg.Length)
        {
            case 2:
                // pawn move: e4
                return ParseSinglePawnMove(alg, board);
            case 3:
                // non-disambiguated piece move: Nf3
                return ParseRegularPieceMove(alg, board);
            case 4:
                // capture
                if (alg[1] == 'x')
                    return ParseRegularCapture(alg, board);
                // non-capture promotion
                if (alg[2] == '=')
                    return ParseRegularPromotion(alg, board);
                // single-disambiguated piece move
                break;
        }
        
        throw new NotationParsingException("Unable to parse notation " + alg);
    }

    private static MoveResult ParseRegularPromotion(string move, Board board)
    {
        byte moveByte = 0b1100_0000;
        byte piece = Parse(move[3]);
        moveByte |= piece;
        int file = Utils.GetFile(move[0]);
        
        int trg = (file, board.turn == 0 ? 7 : 0).GetIndex();
        int src = (file, board.turn == 0 ? 6 : 1).GetIndex();
        
        return new MoveResult([moveByte, (byte)(file | (file << 3))], new Move(trg, src, piece, Flag.Promotion));
    }
    
    private static MoveResult ParseRegularCapture(string move, Board board)
    {
        // piece capture
        if (IsPiece(move[0]))
            return ParseRegularPieceMove(move[0] + move[2..], board);
        
        // pawn capture
        (int File, int rank) target = Utils.ParseSquare(move[2..]);
        (int File, int rank) source = (Utils.GetFile(move[0]), target.rank + board.turn * 2 - 1);

        int trg = target.GetIndex();
        int src = source.GetIndex();

        Flag flag = trg == board.enPassant 
            ? (board.turn == 0 ? Flag.WhiteEnPassant : Flag.BlackEnPassant) 
            : Flag.None;

        int captureFlag = target.File > source.File ? 0 : 0b0100_0000;
        
        return new MoveResult([(byte)(captureFlag | Utils.GetSquareByte(target))], new Move(src, trg, flag: flag));
    }
    
    private static MoveResult ParseRegularPieceMove(string move, Board board)
    {
        (int File, int rank) target = Utils.ParseSquare(move[1..]);
        byte piece = Parse(move[0]);

        MovingPiece pieceData = FindMovingPiece(board, target, piece);

        Move foundMove = new Move(pieceData.Source.GetIndex(), target.GetIndex());

        IEnumerable<byte> bytes = pieceData.FreePiece 
            ? [(byte)(0b10000000 | Utils.GetSquareByte(target))] 
            : [(byte)(0b11100000 | piece), Utils.GetSquareByte(target)];

        return new MoveResult(bytes, foundMove);
    }
    
    private static MovingPiece FindMovingPiece(Board board, (int file, int rank) target, byte piece, Disambiguation d = Disambiguation.None, int dNum = 0)
    {
        if (d == Disambiguation.None)
        {
            ulong allPieces = FindMovingPieceBitboard(board, target);
            if (ulong.PopCount(allPieces) == 1)
                return new MovingPiece(Chess.Bitboards.Utils.FindFileRankFromBitboard(allPieces), true);   
        }

        ulong specifiedPieces = FindMovingPieceBitboard(board, target, piece);

        if (d == Disambiguation.None)
        {
            if (ulong.PopCount(specifiedPieces) == 1)
                return new MovingPiece(Chess.Bitboards.Utils.FindFileRankFromBitboard(specifiedPieces), false);
        }

        throw new NotImplementedException();
    }

    private struct MovingPiece((int file, int rank) source, bool freePiece)
    {
        public readonly (int file, int rank) Source = source;
        public readonly bool FreePiece = freePiece;
    }

    private static ulong FindMovingPieceBitboard(Board board, (int file, int rank) target, byte piece)
    {
        int targetSquare = target.GetIndex();
        
        return TypeOf(piece) switch
        {
            WKnight => Masks.KnightMasks[targetSquare] & board.GetBitboard(board.turn, WKnight),
            WBishop => MagicLookup.Lookup.BishopBitboard(targetSquare, board.AllPieces()) & board.GetBitboard(board.turn, WBishop),
            WRook => MagicLookup.Lookup.RookBitboard(targetSquare, board.AllPieces()) & board.GetBitboard(board.turn, WRook),
            WQueen => MagicLookup.Lookup.QueenBitboards(targetSquare, board.AllPieces()) & board.GetBitboard(board.turn, WQueen),
            WKing => Masks.KingMasks[targetSquare] & board.GetBitboard(board.turn, WKing),
            _ => throw new NotationParsingException($"Could not find moving bitboard of piece {piece}")
        };
    }

    private static ulong FindMovingPieceBitboard(Board board, (int file, int rank) target)
    {
        int targetSquare = target.GetIndex();

        return (Masks.KnightMasks[targetSquare] & board.GetBitboard(board.turn, WKnight))
               | (MagicLookup.Lookup.BishopBitboard(targetSquare, board.AllPieces()) & board.GetBitboard(board.turn, WBishop))
               | (MagicLookup.Lookup.RookBitboard(targetSquare, board.AllPieces()) & board.GetBitboard(board.turn, WRook))
               | (MagicLookup.Lookup.QueenBitboards(targetSquare, board.AllPieces()) & board.GetBitboard(board.turn, WQueen))
               | (Masks.KingMasks[targetSquare] & board.GetBitboard(board.turn, WKing));
    }
    
    private enum Disambiguation
    {
        None,
        File,
        Rank,
        Double
    }

    private static MoveResult ParseSinglePawnMove(string move, Board board)
    {
        (int file, int rank) target = Utils.ParseSquare(move);
        (int file, int rank) source = OffsetSquare(target, yOffset: board.turn == 0 ? -1 : 1);
                
        bool doubleMove = Pieces.TypeOf(board[source]) != Pieces.WPawn;
        if (doubleMove)
            source = OffsetSquare(target, yOffset: board.turn == 0 ? -2 : 2);

        int src = source.GetIndex();
        int trg = target.GetIndex();
        Flag flag = doubleMove ? (board.turn == 0 ? Flag.WhiteDoubleMove : Flag.BlackDoubleMove) : Flag.None;

        byte moveByte = Utils.GetSquareByte(target);
        
        return new(
            [moveByte],
            new(src, trg, flag: flag)
        );
    }

    private static (int file, int rank) OffsetSquare((int file, int rank) square, int xOffset = 0, int yOffset = 0)
    {
        return (square.file + xOffset, square.rank + yOffset);
    }
}