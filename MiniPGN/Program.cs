using ChessLib.API.Display;
using ChessLib.API.Parsing;
using ChessLib.Base;
using MiniPGN.File_Handling;
using MiniPGN.File_Handling.Compression;
using MiniPGN.Minimizer.Game;
using MiniPGN.Utils;
using FileWriter = MiniPGN.File_Handling.Compression.FileWriter;

ChessLib.ChessLib.Init();

string file = "/home/opdragon25/Downloads/lichess_rated_2012.pgn";
string testFile = "TestFile.pgn";

Config config = new Config() { IncludeEncodingDate = true, IncludeGameCount = true };

MPGNFile pgnFile = FileHandler.CompressFile(file, config, false);
Console.WriteLine("Finished conversion");
FileWriter.WriteToFile("/home/opdragon25/Downloads/compressed.mpgn", pgnFile);
//Console.WriteLine(pgnFile.ToHexList);
