using MiniPGN.Utils;

namespace MiniPGN.File_Handling;

public class MPGNFile
{
    public readonly List<byte> body = [];
    public readonly List<byte> header = [];
    public int GameCountIndex;
    public int Count => body.Count;
    public string ToHexList => header.ToHexList() + body.ToHexList();
}