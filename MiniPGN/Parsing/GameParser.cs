namespace MiniPGN.Parsing;
using Chess.Board_Representation;

public abstract class GameParser
{
    protected virtual IEnumerable<byte> ParseGame(string game, Board board)
    {
        board = board.Clone();
        string[] moves = game.Split(' ');
        byte last = 0;

        foreach (string move in moves)
        {
            // skip numbers
            if (move.EndsWith('.'))
                continue;

            MoveResult result = ParseMove(move, board);
            if (result.Move.Source != -1)
                board.MakeMove(result.Move);
            
            foreach (byte b in result.Bytes)
            {
                last = b;
                yield return b;
            }
        }

        if ((last & 0b111_00_111) != 0b111_00_111)
            yield return 0xFF;

        Console.WriteLine(board);
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