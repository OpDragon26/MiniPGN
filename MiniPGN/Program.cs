using ChessLib.API.Display;
using ChessLib.Base;
using MiniPGN.File_Handling;
using MiniPGN.Utils;

List<byte> bytes = FileWriter.GenFileHeader(new Config() {IncludeEncodingDate = true, IncludeGameCount = true});

Console.WriteLine(bytes.ToHexList());
