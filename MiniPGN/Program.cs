using ChessLib.API.Display;
using ChessLib.Base;
using MiniPGN.Utils;

byte[] bytes = "MPGN".ToByteArray();

Console.WriteLine(bytes.ToHexList());
Console.WriteLine(bytes.GetString());