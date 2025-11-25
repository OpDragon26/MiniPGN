using MiniPGN.Chess.Board_Representation;

namespace MiniPGN.Chess.API;

public class Game(Board board)
{
    private readonly List<Node> Nodes = new();
    private Node LastNode => Nodes[^1];

    public Node PlayMove(Move move)
    {
        Board last = board.Clone();
        Nodes.Add(new(last, move));
        board.MakeMove(move);
        return LastNode;
    }
    
    public Node this[Index index] => Nodes[index];
    public Node this[int index] => Nodes[index];
}

public class Node(Board board, Move move, long time = -1)
{
    public Board Board = board;
    public Move Move = move;
    public long Time = time;
}