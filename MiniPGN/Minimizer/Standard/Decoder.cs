using MiniPGN.Chess.Board_Representation;

namespace MiniPGN.Minimizer.Standard;

public class Decoder(IEnumerator<byte> file) : Minimizer.Decoder(file)
{
    protected override MoveResult ParseNextMove(Board board)
    {
        throw new NotImplementedException();
    }

    private Type GetMoveType(byte first)
    {
        if ((first & 0b111_00_111) == 0b111_00_111)
            return Type.Control;
        if ((first & 0b1000_0000) == 0)
            return Type.Pawn;
        if ((first & 0b1100_0000) == 0b10_00_0000)
            return Type.SingleSource;
        if ((first & 0b1110_0000) == 0b110_0_0000)
            return Type.Promotion;
        if ((first & 0b1110_0000) == 0b111_0_0000)
            return (first & 0b000_11_000) == 0 ? Type.Undisambiguated : Type.Disambiguated;

        throw new MoveDecodingException($"Init move not recognized: {first.ToString().PadLeft(8, '0')}");
    }
    
    private enum Type
    {
        Pawn,
        SingleSource,
        Promotion,
        Undisambiguated,
        Disambiguated,
        Control
    }
}