using System.Runtime.InteropServices;
using MiniPGN.Chess.Board_Representation;

namespace MiniPGN.Chess.Parsing;

public static class Display
{
    private static string GetBoardString(Board board, int perspective, PieceTable pieces)
    {
        string boardString = "";
        
        if (perspective == 0)
        {
            boardString += "# a b c d e f g h";
            for (int rank = 7; rank >= 0; rank--)
            {
                boardString += $"\n{rank} ";
                for (int file = 0; file < 8; file++)
                {
                    byte piece = board[file, rank];
                    boardString += $"{pieces[piece]} ";
                }
            }
        }
        else
        {
            boardString += "# h g f e d c b a";
            for (int rank = 0; rank < 8; rank++)
            {
                boardString += $"\n{rank} ";
                for (int file = 7; file >= 0; file--)
                {
                    byte piece = board[file, rank];
                    boardString += $"{pieces[piece]} ";
                }
            }
        }
        return boardString;
    }

    public static string GetBoardString(Board board, int perspective = 0)
    {
        PieceTable actual = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Normal : Windows;
        return GetBoardString(board, perspective, actual);
    }

    public static string GetBitboardString(ulong bitboard, int perspective = 0, char on = '#', char off = ' ')
    {
        string boardString = "";
        
        if (perspective == 0)
        {
            boardString += "# a b c d e f g h";
            for (int rank = 7; rank >= 0; rank--)
            {
                boardString += $"\n{rank} ";
                for (int file = 0; file < 8; file++)
                {
                    bool s = (bitboard & Utils.GetSquare(file, rank)) != 0;
                    boardString += $"{(s ? on : off)} ";
                }
            }
        }
        else
        {
            boardString += "# h g f e d c b a";
            for (int rank = 0; rank < 8; rank++)
            {
                boardString += $"\n{rank} ";
                for (int file = 7; file >= 0; file--)
                {
                    bool s = (bitboard & Utils.GetSquare(file, rank)) != 0;
                    boardString += $"{(s ? on : off)} ";
                }
            }
        }
        return boardString;
    }

    private static readonly PieceTable Normal = new(
        [" ", "♟", "♞", "♝", "♜", "♛", "♚"],
        [" ", "♙", "♘", "♗", "♖", "♕", "♔"]
        );
    
    private static readonly PieceTable Windows = new(
        [" ", "P", "N", "B", "R", "Q", "K"],
        [" ", "p", "n", "b", "r", "q", "k"]
    );
    
    private class PieceTable(string[] wPieces, string[] bPieces)
    {
        public string this[byte piece] => 
            Pieces.ColorOf(piece) == 0 
                ? wPieces[piece] 
                : bPieces[Pieces.TypeOf(piece)];
    }
}