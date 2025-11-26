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
                (int file, int rank) target = Parsing.Utils.ParseSquare(alg);
                (int file, int rank) source = board.turn == 0 ? (target.file, target.rank - 1) : (target.file, target.rank + 1);
                
                bool doubleMove = Pieces.TypeOf(board[source]) != Pieces.WPawn;
                if (doubleMove)
                    source = board.turn == 0 ? (target.file, target.rank - 2) : (target.file, target.rank + 2);
                
                return new(
                    [(byte)(Parsing.Utils.GetSquareByte(target) >> 2)],
                    new(BoardUtils.GetIndex(source), BoardUtils.GetIndex(target), flag: doubleMove ? (board.turn == 0 ? Flag.WhiteDoubleMove : Flag.BlackDoubleMove) : Flag.None)
                );
                break;
        }
        
        throw new NotationParsingException("Unable to parse notation " + alg);
    }
}