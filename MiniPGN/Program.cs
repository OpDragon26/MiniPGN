using MiniPGN.Chess.Board_Representation;
using MiniPGN.Chess.Parsing;

Board test = new Board("rnbqkbnr/ppp1p1pp/5P2/8/2Pp4/8/PP1P1PPP/RNBQKBNR b KQkq c3 0 4");
test.MakeMove(new Move(27, 18, flag: Flag.BlackEnPassant));
Console.WriteLine(test);
Console.WriteLine(Display.GetBitboardString(test.bitboards[Pieces.WPawn]));