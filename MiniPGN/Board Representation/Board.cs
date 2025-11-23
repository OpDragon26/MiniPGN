using System.Runtime.CompilerServices;

namespace MiniPGN.Board_Representation;
using static Utils;
using static Pieces;
using static Parsing.FENParser;
using static Parsing.Display;

public class Board(PieceBoard board, Bitboard bitboards, int turn)
{
    private int turn = turn;
    private PieceBoard board = board;
    public Bitboard bitboards = bitboards;
    
    public Board(string FEN) : this(ParsePieceBoard(FEN.Split(' ')[0]), new(), (FEN.Split(' ')[1][0] == 'w' ? 0 : 1))
    {
        bitboards = FillBitboard(board);
    }
    
    public void MakeMove(Move move)
    {
        bool promotion = move.Flag == Flag.Promotion;
        bool capture = board[move.Target] != Empty; 
            
        byte movingPiece = promotion ? move.Promotion : board[move.Source];

        bitboards[movingPiece, move.Source] = false;
        bitboards[movingPiece, move.Target] = true;
        if (capture)
            bitboards[board[move.Target], move.Target] = false;
        
        board[move.Source] = Empty;
        board[move.Target] = movingPiece;
        
        if (promotion)
            return;

        if ((byte)move.Flag > 1)
            HandleSpecialMove(move);

        turn = 1 - turn;
    }

    private void HandleSpecialMove(Move move)
    {
        switch (move.Flag)
        {
            case Flag.WhiteShortCastle:
            {
                board[7] = Empty;
                board[5] = WRook;
                bitboards[WRook, 7] = false;
                bitboards[WRook, 5] = true;
            } break;
            
            case Flag.WhiteLongCastle:
            {
                board[0] = Empty;
                board[3] = WRook;
                bitboards[WRook, 0] = false;
                bitboards[WRook, 3] = true;
            } break;

            case Flag.BlackShortCastle:
            {
                board[63] = Empty;
                board[61] = BRook;
                bitboards[BRook, 63] = false;
                bitboards[BRook, 61] = true;
            } break;
            
            case Flag.BlackLongCastle:
            {
                board[56] = Empty;
                board[59] = BRook;
                bitboards[BRook, 56] = false;
                bitboards[BRook, 59] = true;
            } break;
            
            case Flag.WhiteEnPassant:
            {
                board[move.Target - 8] = Empty;
                bitboards[BPawn, move.Target - 8] = false;
            } break;

            case Flag.BlackEnPassant:
            {
                board[move.Target + 8] = Empty;
                bitboards[WPawn, move.Target + 8] = false;
            } break;
        }
    }

    public override string ToString()
    {
        return GetBoardString(this);
    }

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

    public Board Clone()
    {
        return new Board(board, bitboards, turn);
    }
}

[InlineArray(64)]
public struct PieceBoard
{
    public byte piece;
    
    public byte this[int file, int rank]
    {
        get => this[GetIndex(file, rank)];
        set => this[GetIndex(file, rank)] = value;
    }
}

[InlineArray(15)]
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