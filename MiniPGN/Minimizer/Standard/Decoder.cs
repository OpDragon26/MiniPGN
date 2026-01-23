using MiniPGN.Chess;
using MiniPGN.Chess.Board_Representation;
using MiniPGN.Parsing;
using static MiniPGN.Parsing.DisambiguationUtils;

namespace MiniPGN.Minimizer.Standard;

public class Decoder(IEnumerator<byte> file) : Minimizer.Decoder(file)
{
    protected override MoveResult ParseNextMove(Board board)
    {
        Sign moveSign = GetMoveSign(File.Current);
        
        return moveSign switch
        {
            Sign.Pawn => ParsePawnMove(board),
            Sign.SingleSource => ParseSingleSourceMove(board),
            Sign.Promotion => ParsePromotion(board),
            Sign.Undisambiguated => ParseUndisambiguatedMove(board),
            Sign.Disambiguated => ParseDisambiguatedMove(board),
            _ => throw new NotImplementedException()
        };
    }

    private MoveResult ParseDisambiguatedMove(Board board)
    {
        byte[] bytes = File.Extract(3).ToArray();

        Disambiguation disambiguation = ((bytes[0] >> 3) & 0b11) switch
        {
            0b10 => Disambiguation.File,
            0b01 => Disambiguation.Rank,
            0b11 => Disambiguation.Double,
            _ => throw new NotationParsingException($"Disambiguation not recognized: {bytes[0].GetBinaryString()}")
        };

        byte piece = (byte)(bytes[0] & 0b111);
        
        (int file, int rank) source = bytes[1].AsSquare();
        (int file, int rank) target = bytes[2].AsSquare();

        int srcIndex = source.GetIndex();
        int trgIndex = target.GetIndex();

        bool capture = board.Occupied(target);
        
        Move move = new Move(srcIndex, trgIndex);
        string disaStr = disambiguation switch
        {
            Disambiguation.File => source.file.ToFile().ToString(),
            Disambiguation.Rank => (source.rank + 1).ToString(),
            Disambiguation.Double => source.SquareString(),
            _ => throw new Exception("no")
        };
        string moveStr = $"{piece.ToPiece()}{disaStr}{(capture ? "x" : "")}{target.SquareString()}";

        return new MoveResult(move, moveStr);
    }

    private MoveResult ParseUndisambiguatedMove(Board board)
    {
        byte[] bytes = File.Extract(2).ToArray();
        
        byte piece = (byte)(bytes[0] & 0b111);

        (int file, int rank) target = bytes[1].AsSquare();
        MovingPiece pieceData = FindMovingPiece(board, target, piece);

        int srcIndex = target.GetIndex();
        int trgIndex = pieceData.Source.GetIndex();

        if (IsCastling(pieceData.Source, target, piece))
        {
            string moveStr = pieceData.Source.file < target.file
                ? "O-O"
                : "O-O-O";
            Flag flag = pieceData.Source.file < target.file
                ? (board.turn == 0 ? Flag.WhiteShortCastle : Flag.BlackShortCastle)
                : (board.turn == 0 ? Flag.WhiteLongCastle : Flag.BlackLongCastle);
            Move move = new Move(srcIndex, trgIndex, flag: flag);

            return new MoveResult(move, moveStr);
        }
        else
        {
            bool capture = board.Occupied(target);
            char pieceChar = piece.ToPiece();
            
            string moveStr = capture
                ? $"{pieceChar}x{target.SquareString()}"
                : $"{pieceChar}{target.SquareString()}";
            Move move = new Move(srcIndex, trgIndex);

            return new MoveResult(move, moveStr);
        }
    }

    private bool IsCastling((int file, int rank) source, (int file, int rank) target, byte piece)
    {
        return Pieces.TypeOf(piece) == Pieces.WKing
               && Math.Abs(source.file - target.file) > 1;
    }

    private MoveResult ParsePromotion(Board board)
    {
        byte[] bytes = File.Extract(2).ToArray();

        byte piece = (byte)(bytes[0] & 0b111);

        (int srcFile, int trgFile) = bytes[1].AsSquare();
        bool capture = srcFile == trgFile;

        (int file, int rank) source = (srcFile, board.turn == 0 ? 6 : 1);
        (int file, int rank) target = (trgFile, board.turn == 0 ? 7 : 0);

        int srcIndex = source.GetIndex();
        int trgIndex = target.GetIndex();

        Move move = new Move(srcIndex, trgIndex, piece, Flag.Promotion);
        string moveStr = capture
            ? $"{srcFile.ToFile()}x{target.SquareString()}={piece.ToPiece()}"
            : $"{target.SquareString()}={piece.ToPiece()}";

        return new MoveResult(move, moveStr);
    }

    private MoveResult ParseSingleSourceMove(Board board)
    {
        byte b = File.Next();
        (int file, int rank) target = b.AsSquare();
        
        // find source
        MovingPiece pieceData = FindFreePiece(board, target);

        int srcIndex = pieceData.Source.GetIndex();
        int trgIndex = target.GetIndex();

        char piece = pieceData.Piece.ToPiece();
        bool capture = board.Occupied(target);
        
        Move move = new Move(srcIndex, trgIndex);
        string moveStr = $"{piece}{(capture ? "x" : "")}{target.SquareString()}";

        return new MoveResult(move, moveStr);
    }

    private MoveResult ParsePawnMove(Board board)
    {
        byte b = File.Next();

        (int file, int rank) target = b.AsSquare();
        ulong targetBitboard = target.Bitboard();
        int offset = board.turn * 2 - 1;
        
        // can be forward or capture from lower file
        if ((b & 0b0100_0000) == 0)
        {
            if ((board.AllPieces() & targetBitboard) == 0) // forward move
            {
                ulong pawns = board.GetBitboard(board.turn, Pieces.WPawn);

                (int file, int rank) source = target.OffsetBy((0, -offset));
                if ((pawns & source.Bitboard()) == 0)
                    source = source.OffsetBy((0, -offset));

                int srcIndex = source.GetIndex();
                int trgIndex = target.GetIndex();

                Move moveObj = new Move(srcIndex, trgIndex);
                string moveStr = target.SquareString();
                
                return new MoveResult(moveObj, moveStr);
            }
            else // capture from lower file
            {
                (int file, int rank) source = target.OffsetBy((-1, -offset));
                
                int srcIndex = source.GetIndex();
                int trgIndex = target.GetIndex();
                
                Move moveObj = new Move(srcIndex, trgIndex);
                string moveStr = $"{((byte)source.file).ToFile()}x{target.SquareString()}";
                
                return new MoveResult(moveObj, moveStr);
            }
        }
        else // capture from higher file
        {
            (int file, int rank) source = target.OffsetBy((1, -offset));
                
            int srcIndex = source.GetIndex();
            int trgIndex = target.GetIndex();
                
            Move moveObj = new Move(srcIndex, trgIndex);
            string moveStr = $"{((byte)source.file).ToFile()}x{target.SquareString()}";
                
            return new MoveResult(moveObj, moveStr);
        }
        
    }

    private Sign GetMoveSign(byte first)
    {
        if ((first & 0b111_00_111) == 0b111_00_111)
            return Sign.Control;
        if ((first & 0b1000_0000) == 0)
            return Sign.Pawn;
        if ((first & 0b1100_0000) == 0b10_00_0000)
            return Sign.SingleSource;
        if ((first & 0b1110_0000) == 0b110_0_0000)
            return Sign.Promotion;
        if ((first & 0b1110_0000) == 0b111_0_0000)
            return (first & 0b000_11_000) == 0 ? Sign.Undisambiguated : Sign.Disambiguated;

        throw new MoveDecodingException($"Move signature not recognized: {first.ToString().PadLeft(8, '0')}");
    }

    private enum Sign
    {
        Pawn,
        SingleSource,
        Promotion,
        Undisambiguated,
        Disambiguated,
        Control
    }
}