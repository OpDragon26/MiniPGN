namespace MiniPGN.Board_Representation;

public static class Pieces
{
    public const byte WPawn = 0b0000;
    public const byte WKnight = 0b0001;
    public const byte WBishop = 0b0010;
    public const byte WRook = 0b0011;
    public const byte WQueen = 0b0100;
    public const byte WKing = 0b0101; // 5
    
    public const byte BPawn = 0b1000; // 8
    public const byte BKnight = 0b1001;
    public const byte BBishop = 0b1010;
    public const byte BRook = 0b1011;
    public const byte BQueen = 0b1100;
    public const byte BKing = 0b1101; // 13

    public static int ColorOf(byte piece)
    {
        return piece >> 3;
    }
    
    public static int TypeOf(byte piece)
    {
        return piece & 0b111;
    }
}