using MiniPGN.Chess.Board_Representation;

namespace MiniPGN.Chess.Parsing;

public static class FENParser
{
    public static PieceBoard ParsePieceBoard(string FENBoard)
    {
        PieceBoard board = new();
        string[] ranks = FENBoard.Split('/');
        
        for (int r = 0; r < 8; r++) // for each rank
        {
            int file = 0;

            for (int c = 0; c < ranks[r].Length; c++) // for each character
            {
                if (int.TryParse(ranks[r][c].ToString(), out int v)) // if the character is a number
                    for (int i = 0; i < v; i++) // fill that many empty squares
                        board[file++, 7 - r] = Pieces.Empty;
                else
                {
                    byte piece = Pieces.Parse(ranks[r][c]);
                    board[file++, 7 - r] = piece;
                }
            }
        }
            
        return board;
    }

    public static Bitboard FillBitboard(PieceBoard board)
    {
        Bitboard bitboards = new();

        for (int square = 0; square < 64; square++)
            bitboards[board[square], square] = true;
        
        return bitboards;
    }
}