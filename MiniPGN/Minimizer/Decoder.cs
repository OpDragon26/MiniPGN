using MiniPGN.Chess.Board_Representation;

namespace MiniPGN.Minimizer;

public abstract class Decoder(IEnumerator<byte> file)
{
    protected readonly IEnumerator<byte> File = file;
    
    public virtual string ParseGame(Board board)
    {
        List<string> fullGame = new();
        int moves = 1;
        
        do
        {
            if (++moves % 2 == 0)
                fullGame.Add($"{moves}.");

            MoveResult move = ParseNextMove(board);

            Console.WriteLine(move.Str);
            
            if (move.Move is not null)
                board.MakeMove(move.Move);
            if (move.Str is not null)
                fullGame.Add(move.Str);
        } while (File.Current != 0xFF);

        return string.Join(' ', fullGame);
    }

    public string ParseGame()
    {
        return ParseGame(Board.NewStartingBoard());
    }

    protected abstract MoveResult ParseNextMove(Board board);

    protected readonly struct MoveResult(Move? move, string? str)
    {
        public readonly Move? Move = move;
        public readonly string? Str = str;
    }
}