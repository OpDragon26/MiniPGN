namespace MiniPGN.Minimizer.Standard;
using Chess.Board_Representation;
using Parsing;
using BoardUtils = Chess.Board_Representation.Utils;

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
                break;
            
            case 3:
                // non-disambiguated piece move: Nf3
                
            break;
        }
        
        throw new NotationParsingException("Unable to parse notation " + alg);
    }

    private static MoveResult ParseRegularPieceMove(string move, Board board)
    {
        throw new NotImplementedException();
    }

    private static MoveResult ParseSinglePawnMove(string move, Board board)
    {
        (int file, int rank) target = Parsing.Utils.ParseSquare(move);
        (int file, int rank) source = OffsetSquare(target, yOffset: board.turn == 0 ? -1 : 1);
                
        bool doubleMove = Pieces.TypeOf(board[source]) != Pieces.WPawn;
        if (doubleMove)
            source = OffsetSquare(target, yOffset: board.turn == 0 ? -2 : 2);

        int src = BoardUtils.GetIndex(source);
        int trg = BoardUtils.GetIndex(target);
        Flag flag = doubleMove ? (board.turn == 0 ? Flag.WhiteDoubleMove : Flag.BlackDoubleMove) : Flag.None;

        byte moveByte = Parsing.Utils.GetSquareByte(target);
        
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