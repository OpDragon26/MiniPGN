using System.Runtime.CompilerServices;

namespace MiniPGN.Board_Representation;
using static Utils;

public class Board
{
    private PieceBoard board;
    
    public byte this[int index]
    {
        get => board[index];
        set => board[index] = value;
    }

    public byte this[int file, int rank]
    {
        get => board[GetIndex(file, rank)];
        set => board[GetIndex(file, rank)] = value;
    }
}

[InlineArray(64)]
public struct PieceBoard
{
    public byte piece;
}

[InlineArray(14)]
public struct Bitboard
{
    public ulong bitboard;

    public bool this[byte piece, int square]
    {
        get => (this[piece] & GetSquare(square)) != 0;
        set
        {
            if (value)
                this[piece] |= GetSquare(square);
            else
                this[piece] &= ~GetSquare(square);
        }
    }
}