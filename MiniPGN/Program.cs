using MiniPGN.Chess;
using MiniPGN.Chess.Bitboards;
using MiniPGN.Chess.Board_Representation;
using MiniPGN.Chess.Parsing;
using MiniPGN.Minimizer;
using Type = MiniPGN.Minimizer.Type;
using BitboardUtils = MiniPGN.Chess.Bitboards.Utils;

Init.Start();

byte[] byteFile = Encoder.Active.Encode(new(Type.Standard, Metadata.Exclude)
{
    file =
    [
        "[Event \"Rated Blitz game\"]",
        "[Site \"https://lichess.org/n3p0bgc7\"]",
        "[White \"georgek\"]",
        "[Black \"b777\"]",
        "[Black \"TheWeebles\"]",
        "[Result \"1-0\"]",
        "[UTCDate \"2012.12.31\"]",
        "[UTCTime \"23:12:39\"]",
        "[WhiteElo \"1554\"]",
        "[BlackElo \"1429\"]",
        "[WhiteRatingDiff \"+7\"]",
        "[BlackRatingDiff \"-7\"]",
        "[ECO \"B01\"]",
        "[Opening \"Scandinavian Defense: Mieses-Kotroc Variation\"]",
        "[TimeControl \"300+2\"]",
        "[Termination \"Time forfeit\"]",
        "1. e4 e5 2. Ne2 d5 3. Nbc3 dxe4 4. Nxe4 Nc6 5. N4c3 e4 6. f3 exf3 7. Nd4 fxg2 8. Nb3 gxh1=N 9. Be2 Ng3 10. a3 Nf6 11. h3 Nfe4 12. a4 Ng5 13. a5 Nb4 14. a6 Nxa6 15. Ra2 Nc5 16. Ra3 Ng5e4"
    ]
});

Console.WriteLine(MiniPGN.Minimizer.Utils.ToString(byteFile));