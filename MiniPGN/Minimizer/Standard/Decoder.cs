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
            _ => throw new NotImplementedException()
        };
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