using ChessLib.API.Display;
using ChessLib.Base;
using MiniPGN.File_Handling;
using MiniPGN.Utils;

byte[] bytes = FileWriter.GenFileHeader(new Config() {IncludeEncodingDate = true, IncludeGameCount = true}).ToArray();

Console.WriteLine(bytes.ToHexList());
