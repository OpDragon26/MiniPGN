namespace MiniPGN.Chess.Board_Representation;

public class Move(int source, int target, byte promotion = 0b0000, Flag flag = Flag.None)
{
    public readonly int Source = source;
    public readonly int Target = target;
    public readonly byte Promotion = promotion;
    public readonly Flag Flag = flag;
}

public enum Flag : byte
{
    None,
    Promotion,
    WhiteEnPassant,
    BlackEnPassant,
    WhiteShortCastle,
    WhiteLongCastle,
    BlackShortCastle,
    BlackLongCastle
}