using MiniPGN.Chess;
using MiniPGN.Chess.Bitboards;

namespace MiniPGN.Minimizer.Standard;
using Chess.Board_Representation;
using Parsing;
using static Chess.Board_Representation.Pieces;

public class Parser : GameParser
{
    private static readonly MoveResult WhiteShortCastle = new([0b111_00_110, 0b00_110_000], new( 4,  6, flag: Flag.WhiteShortCastle));
    private static readonly MoveResult BlackShortCastle = new([0b111_00_110, 0b00_110_111], new(60, 62, flag: Flag.BlackShortCastle));
    private static readonly MoveResult WhiteLongCastle = new([0b111_00_110, 0b00_010_000], new( 4,  6, flag: Flag.WhiteLongCastle));
    private static readonly MoveResult BlackLongCastle = new([0b111_00_110, 0b00_010_111], new(60, 62, flag: Flag.BlackLongCastle));
    
    protected override MoveResult ParseMove(string notation, Board board)
    {
        if (notation.EndsWith('#') || notation.EndsWith('+'))
            return ParseMove(notation[..^1], board);

        if (notation.Equals("O-O"))
            return board.turn == 0 ? WhiteShortCastle : BlackShortCastle;
        if (notation.Equals("O-O-O"))
            return board.turn == 0 ? WhiteLongCastle : BlackLongCastle;

        if (notation.Equals("1-0"))
            return new([0b111_10_111], new(-1,-1));
        if (notation.Equals("0-1"))
            return new([0b111_01_111], new(-1,-1));
        if (notation.Equals("1/2-1/2"))
            return new([0b111_00_111], new(-1,-1));
        
        switch (notation.Length)
        {
            case 2:
                // pawn move: e4
                return ParseSinglePawnMove(notation, board);
            case 3:
                // non-disambiguated piece move: Nf3
                return ParseRegularPieceMove(notation, board);
            case 4:
                // capture
                if (notation[1] == 'x')
                    return ParseRegularCapture(notation, board);
                // non-capture promotion
                if (notation[2] == '=')
                    return ParseRegularPromotion(notation, board);
                // single-disambiguated piece move
                return ParseSingleDisambiguatedPieceMove(notation, board);
            
            case 5:
                // single disambiguated capture
                if (notation[2] == 'x')
                    return ParseSingleDisambiguatedCapture(notation, board);
                // doubly disambiguated move
                return ParseDoublyDisambiguatedPieceMove(notation);
            case 6:
                // doubly disambiguated capture
                if (notation[3] == 'x')
                    return ParseDoublyDisambiguatedCapture(notation);
                // capture promotion
                return ParseCapturePromotion(notation, board);
        }
        
        throw new NotationParsingException("Unable to parse notation " + notation);
    }

    private static MoveResult ParseCapturePromotion(string move, Board board)
    {
        byte piece = Parse(move[5]);
        byte moveByte = (byte)(0b110_00_000 | piece);
        
        (int file, int rank) target = Utils.ParseSquare(move[2..4]);
        int srcFile = move[0].AsFile();

        int src = (srcFile, board.turn == 0 ? 6 : 1).GetIndex();
        int trg = target.GetIndex();

        return new MoveResult([moveByte, (byte)(target.file | (srcFile << 3))], new(src, trg, piece, Flag.Promotion));
    }
    
    private static MoveResult ParseDoublyDisambiguatedCapture(string move)
    {
        return ParseDoublyDisambiguatedPieceMove(move[..3] + move[4..]);
    }
    
    private static MoveResult ParseDoublyDisambiguatedPieceMove(string move)
    {
        (int file, int rank) source = Utils.ParseSquare(move[1..3]);
        (int file, int rank) target = Utils.ParseSquare(move[3..]);

        byte piece = Parse(move[0]);
        byte moveByte = (byte)(0b111_11_000 | piece);

        int src = source.GetIndex();
        int trg = target.GetIndex();
        
        return new MoveResult([moveByte, source.ToByte(), target.ToByte()], new(src, trg));
    }
    
    private static MoveResult ParseSingleDisambiguatedCapture(string move, Board board)
    {
        return ParseSingleDisambiguatedPieceMove(move[..2] + move[3..], board);
    }
    
    private static MoveResult ParseSingleDisambiguatedPieceMove(string move, Board board)
    {
        (int file, int rank) target = Utils.ParseSquare(move[2..]);
        byte piece = Parse(move[0]);
        
        // file disambiguation
        if (move[1].IsFile())
        {
            MovingPiece pieceData = FindMovingPiece(board, target, piece, Disambiguation.File, dNum: move[1].AsFile());
            
            int src = pieceData.Source.GetIndex();
            int trg = target.GetIndex();

            byte moveByte = (byte)(0b111_10_000 | piece);
            
            return new MoveResult([moveByte, pieceData.Source.ToByte(), target.ToByte()], new Move(src, trg));
        }
        else
        {
            MovingPiece pieceData = FindMovingPiece(board, target, piece, Disambiguation.Rank, move[1].ToNum());
            
            int src = pieceData.Source.GetIndex();
            int trg = target.GetIndex();

            byte moveByte = (byte)(0b111_01_000 | piece);
            
            return new MoveResult([moveByte, pieceData.Source.ToByte(), target.ToByte()], new Move(src, trg));
        }
    }

    private static MoveResult ParseRegularPromotion(string move, Board board)
    {
        byte moveByte = 0b1100_0000;
        byte piece = Parse(move[3]);
        moveByte |= piece;
        int file = move[0].AsFile();
        
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
        (int File, int rank) source = (move[0].AsFile(), target.rank + board.turn * 2 - 1);

        int trg = target.GetIndex();
        int src = source.GetIndex();

        Flag flag = trg == board.enPassant 
            ? (board.turn == 0 ? Flag.WhiteEnPassant : Flag.BlackEnPassant) 
            : Flag.None;

        int captureFlag = target.File > source.File ? 0 : 0b0100_0000;
        
        return new MoveResult([(byte)(captureFlag | target.ToByte())], new Move(src, trg, flag: flag));
    }
    
    private static MoveResult ParseRegularPieceMove(string move, Board board)
    {
        (int File, int rank) target = Utils.ParseSquare(move[1..]);
        byte piece = Parse(move[0]);

        MovingPiece pieceData = FindMovingPiece(board, target, piece);

        Move foundMove = new Move(pieceData.Source.GetIndex(), target.GetIndex());

        IEnumerable<byte> bytes = pieceData.FreePiece 
            ? [(byte)(0b1000_0000 | target.ToByte())] 
            : [(byte)(0b1110_0000 | piece), target.ToByte()];

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

        if (d == Disambiguation.File)
            specifiedPieces &= Chess.Bitboards.Utils.GetFile(dNum);
        else if (d == Disambiguation.Rank)
            specifiedPieces &= Chess.Bitboards.Utils.GetRank(dNum);
        
        
        if (ulong.PopCount(specifiedPieces) == 1)
            return new MovingPiece(Chess.Bitboards.Utils.FindFileRankFromBitboard(specifiedPieces), false);

        if (ulong.PopCount(specifiedPieces) == 0)
            throw new NotationParsingException("Could not find moving piece");
        throw new NotationParsingException("Insufficient disambiguation");
    }

    private struct MovingPiece((int file, int rank) source, bool freePiece)
    {
        public readonly (int file, int rank) Source = source;
        public readonly bool FreePiece = freePiece;
    }

    private static ulong FindMovingPieceBitboard(Board board, (int file, int rank) target, byte piece)
    {
        int targetSquare = target.GetIndex();
        
        ulong potentialPieces = TypeOf(piece) switch
        {
            WKnight => Masks.KnightMasks[targetSquare] & board.GetBitboard(board.turn, WKnight),
            WBishop => MagicLookup.Lookup.BishopBitboard(targetSquare, board.AllPieces()) & board.GetBitboard(board.turn, WBishop),
            WRook => MagicLookup.Lookup.RookBitboard(targetSquare, board.AllPieces()) & board.GetBitboard(board.turn, WRook),
            WQueen => MagicLookup.Lookup.QueenBitboards(targetSquare, board.AllPieces()) & board.GetBitboard(board.turn, WQueen),
            WKing => Masks.KingMasks[targetSquare] & board.GetBitboard(board.turn, WKing),
            _ => throw new NotationParsingException($"Could not find moving bitboard of piece {piece}")
        };
        
        return FilterPins(board, target, potentialPieces);
    }

    private static ulong FindMovingPieceBitboard(Board board, (int file, int rank) target)
    {
        int targetSquare = target.GetIndex();

        ulong potentialPieces = (Masks.KnightMasks[targetSquare] & board.GetBitboard(board.turn, WKnight))
               | (MagicLookup.Lookup.BishopBitboard(targetSquare, board.AllPieces()) & board.GetBitboard(board.turn, WBishop))
               | (MagicLookup.Lookup.RookBitboard(targetSquare, board.AllPieces()) & board.GetBitboard(board.turn, WRook))
               | (MagicLookup.Lookup.QueenBitboards(targetSquare, board.AllPieces()) & board.GetBitboard(board.turn, WQueen))
               | (Masks.KingMasks[targetSquare] & board.GetBitboard(board.turn, WKing));
        
        return FilterPins(board, target, potentialPieces);
    }
    
    private enum Disambiguation
    {
        None,
        File,
        Rank,
        Double
    }

    private static ulong FilterPins(Board board, (int file, int rank) target, ulong pieces)
    {
        ulong t = target.Bitboard();
        
        ulong pinnedPieces = board
            .GetPinState()
            .Paths // list of pieces which are pinned and their movement paths
            .Where(p => (p.Value & t) == 0) // where the path does not contain the target square
            .Select(p => p.Key) // coordinate of the pinned piece
            .Aggregate((a, b) => a | b); // bitboard of pieces which are pinned and cannot move to the target square
        
        return pieces & ~pinnedPieces; // remove those pieces from the ones found previously
    }

    private static MoveResult ParseSinglePawnMove(string move, Board board)
    {
        (int file, int rank) target = Utils.ParseSquare(move);
        (int file, int rank) source = OffsetSquare(target, yOffset: board.turn == 0 ? -1 : 1);
                
        bool doubleMove = TypeOf(board[source]) != WPawn;
        if (doubleMove)
            source = OffsetSquare(target, yOffset: board.turn == 0 ? -2 : 2);

        int src = source.GetIndex();
        int trg = target.GetIndex();
        Flag flag = doubleMove ? (board.turn == 0 ? Flag.WhiteDoubleMove : Flag.BlackDoubleMove) : Flag.None;

        byte moveByte = target.ToByte();
        
        return new([moveByte], new(src, trg, flag: flag));
    }

    private static (int file, int rank) OffsetSquare((int file, int rank) square, int xOffset = 0, int yOffset = 0)
    {
        return (square.file + xOffset, square.rank + yOffset);
    }
}