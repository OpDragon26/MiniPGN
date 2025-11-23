using MiniPGN.Bitwise_Storage;
using MiniPGN.Board_Representation;
using MiniPGN.Parsing;

Board test = new Board("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
test.MakeMove(new Move(4, 6, flag: Flag.WhiteShortCastle));
Console.WriteLine(test);
Console.WriteLine(Display.GetBitboardString(test.bitboards[Pieces.WRook]));