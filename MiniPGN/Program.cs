using MiniPGN.Chess;
using MiniPGN.Chess.Bitboards;
using MiniPGN.Chess.Board_Representation;
using MiniPGN.Chess.Parsing;
using MiniPGN.Minimizer;
using Type = MiniPGN.Minimizer.Type;
using BitboardUtils = MiniPGN.Chess.Bitboards.Utils;

Init.Start();

byte[] byteFile = Encoder.Active.Encode(new(Type.Standard, Metadata.Include)
{
    file =
    [
        "[Event \"Rated Blitz game\"]",
        "[Site \"https://lichess.org/j1dkb5dw\"]",
        "[Round \"?\"]",
        "[White \"H___N\"]",
        "[Black \"TheWeebles\"]",
        "[Result \"0-1\"]",
        "[UTCDate \"2013.01.01\"]",
        "[UTCTime \"00:56:46\"]",
        "[WhiteElo \"1595\"]",
        "[BlackElo \"1546\"]",
        "[WhiteRatingDiff \"-37\"]",
        "[BlackRatingDiff \"+16\"]",
        "[ECO \"C25\"]",
        "[Opening \"Vienna Game: Max Lange Defense\"]",
        "[TimeControl \"480+0\"]",
        "[Termination \"Normal\"]",
        "1. e4 e6 2. Nf3"
    ]
});

Console.WriteLine(MiniPGN.Minimizer.Utils.ToString(byteFile));