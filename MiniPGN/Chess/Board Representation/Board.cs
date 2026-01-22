using System.Runtime.CompilerServices;

namespace MiniPGN.Chess.Board_Representation;
using static Utils;
using static Pieces;
using static Parsing.FENParser;
using static Parsing.Display;

public class Board(PieceBoard board, Bitboard bitboards, int turn)
{
    public int turn = turn;
    private PieceBoard board = board;
    public Bitboard bitboards = bitboards;
    
    public int enPassant = -1;
    
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

        if (!promotion)
        {
            enPassant = -1;
            if ((byte)move.Flag > 1)
                HandleSpecialMove(move);
        }
        
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

            case Flag.WhiteDoubleMove:
            {
                enPassant = move.Target - 8;
            } break;
            
            case Flag.BlackDoubleMove:
            {
                enPassant = move.Target + 8;
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
    
    public byte this[(int file, int rank) square]
    {
        get => board[GetIndex(square.file, square.rank)];
        set => board[GetIndex(square.file, square.rank)] = value;
    }

    public bool Occupied((int file, int rank) square)
    {
        ulong s = square.Bitboard();
        return (s & AllPieces()) != 0;
    }

    public Board Clone()
    {
        return new Board(board, bitboards, turn);
    }

    public static Board NewStartingBoard()
    {
        return new Board("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
    }

    public ulong AllPieces()
    {
        return bitboards[WPawn] | bitboards[BPawn] | bitboards[WKnight] | bitboards[BKnight] | bitboards[WBishop] |
               bitboards[BBishop] | bitboards[WRook] | bitboards[BRook] | bitboards[WQueen] | bitboards[BQueen] |
               bitboards[WKing] | bitboards[BKing];
    }

    public ulong AllPieces(int side)
    {
        return side == 0
            ? bitboards[WPawn] | bitboards[WKnight] | bitboards[WBishop] | bitboards[WRook] | bitboards[WQueen] | bitboards[WKing]
            : bitboards[BPawn] | bitboards[BKnight] | bitboards[BBishop] | bitboards[BRook] | bitboards[BQueen] | bitboards[BKing];
    }

    public ulong GetBitboard(int side, byte piece)
    {
        return bitboards[piece | (side << 3)];
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