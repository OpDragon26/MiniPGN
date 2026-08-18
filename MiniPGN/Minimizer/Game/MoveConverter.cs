using ChessLib.API.Parsing;
using ChessLib.Base;
using ChessLib.Base.Utils;
using ChessLib.Utils;
using MiniPGN.Utils;
using ThrowHelper = MiniPGN.Utils.ThrowHelper;
using static ChessLib.API.Parsing.ParsingUtils;

namespace MiniPGN.Minimizer.Game;

public static class MoveConverter
{
    public static byte[] Convert(this Move move, Board board)
    {
        byte code = 0;
        
        if (move.IsPawnRegular(board))
        {
            if (move.IsFromLeft())
                code = 0b01_000000;
            code |= move.Target.ToByteCoordinate();
            return [code];
        }

        if (move.IsPromotion)
        {
            code = 0b110_00000;
            code |= move.Promotion;
            byte position = (move.Source.FileOf(), move.Target.FileOf()).ToByteCoordinate();
            return [code, position];
        }

        if (ConverterUtils.IsSingleMove(board, move))
        {
            code = 0b10_000000;
            code |= move.Target.ToByteCoordinate();
            return [code];
        }
        
        Disambiguation disambiguation = FindMinimalDisambiguation(board, move);
        byte piece = board[move.Source];
        code = 0b111_00000;
        code |= piece;
        
        if (disambiguation == Disambiguation.None)
        {
            byte position = move.Target.ToByteCoordinate();
            return [code, position];
        }
        else
        {
            code |= disambiguation switch {
                Disambiguation.File   => 0b000_10_000,
                Disambiguation.Rank   => 0b000_01_000,
                Disambiguation.Double => 0b000_11_000,
                _ => throw new ThrowHelper.MoveConverterException("Failed to parse move")
            };
            byte sourcePosition = move.Source.ToByteCoordinate();
            byte targetPosition = move.Target.ToByteCoordinate();
            return [code, sourcePosition, targetPosition];
        }
        
        throw new ThrowHelper.MoveConverterException("Failed to parse move");
    }

    static bool IsFromLeft(this Move move)
    {
        return move.Source.FileOf() < move.Target.FileOf();
    }
    
    static bool IsPawnRegular(this Move move, Board board)
    {
        return board[move.Source].IsType(Pieces.Pawn)
               && !move.IsPromotion;
    }
}