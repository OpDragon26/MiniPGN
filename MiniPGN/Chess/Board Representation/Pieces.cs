namespace MiniPGN.Chess.Board_Representation;

public static class Pieces
{
    public const byte WPawn = 0b0001;
    public const byte WKnight = 0b0010;
    public const byte WBishop = 0b0011;
    public const byte WRook = 0b0100;
    public const byte WQueen = 0b0101;
    public const byte WKing = 0b0110; // 6

    public const byte BPawn = 0b1001; // 9
    public const byte BKnight = 0b1010;
    public const byte BBishop = 0b1011;
    public const byte BRook = 0b1100;
    public const byte BQueen = 0b1101;
    public const byte BKing = 0b1110; // 14

    public const byte Empty = 0;

    public static int ColorOf(byte piece)
    {
        return piece >> 3;
    }

    public static int TypeOf(byte piece)
    {
        return piece & 0b111;
    }

    public static byte Parse(char p)
    {
        return pieceDict[p];
    }

    private static readonly Dictionary<char, byte> pieceDict = new()
    {
        { 'P' , WPawn },
        { 'N' , WKnight },
        { 'B' , WBishop },
        { 'R' , WRook },
        { 'Q' , WQueen },
        { 'K' , WKing },
        
        { 'p' , BPawn },
        { 'n' , BKnight },
        { 'b' , BBishop },
        { 'r' , BRook },
        { 'q' , BQueen },
        { 'k' , BKing },
    };
}