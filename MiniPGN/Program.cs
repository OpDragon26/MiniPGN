using MiniPGN.Chess;
using MiniPGN.Minimizer;
using Type = MiniPGN.Minimizer.Type;

Init.Start();

byte[] byteFile = EncodingHandler.Active.Encode(new(Type.Standard, Metadata.Exclude, true, true)
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
        "1. e4 d5 2. exd5 Nc6"
    ]
});

Console.WriteLine(MiniPGN.Minimizer.Utils.ToString(byteFile));

EncodingHandler.Active.Decode([0x4D, 0x50, 0x47, 0x4E, 0x76, 0x30, 0x30, 0x2E, 0x30, 0x31, 0x53, 0x45, 0x01, 0x07, 0xEA, 0x01, 0x16, 0x08, 0x32, 0x16, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0xFF, 0xFF, 0x23, 0x1C, 0x5C, 0x95, 0xFF]);