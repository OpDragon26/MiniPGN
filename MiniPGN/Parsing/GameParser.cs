namespace MiniPGN.Parsing;
using Chess.Board_Representation;

public abstract class GameParser
{
    public virtual IEnumerable<byte> ParseGame(string game, Board board)
    {
        board = board.Clone();
        string[] moves = game.Split(' ');

        foreach (string move in moves)
        {
            // skip numbers
            if (move[1] == '.')
                continue;

            MoveResult result = ParseMove(move, board);
            board.MakeMove(result.Move);
            foreach (byte b in result.Bytes)
                yield return b;
        }
    }
    
    public virtual IEnumerable<byte> ParseGame(string game)
    {
        return ParseGame(game, Board.NewStartingBoard());
    }
    
    protected abstract MoveResult ParseMove(string move, Board board);
    
    protected struct MoveResult(IEnumerable<byte> bytes, Move move)
    {
        public readonly IEnumerable<byte> Bytes = bytes;
        public readonly Move Move = move;
    }
}