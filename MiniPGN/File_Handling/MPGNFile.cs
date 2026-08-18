using MiniPGN.Utils;

namespace MiniPGN.File_Handling;

public class MPGNFile
{
    public readonly List<byte> body = [];
    public readonly List<byte> header = [];
    public int GameCountIndex;
    public int Count => header.Count + body.Count;
    public string ToHexList => header.ToHexList() + " " + body.ToHexList();
    
    public void AddGameCount(ulong gameCount)
    {
        if (GameCountIndex != -1)
        {
            byte[] gameCountBytes = gameCount.ToBytes();
            for (int i = 0; i < 8; i++)
                header[GameCountIndex + i] = gameCountBytes[i];
        }
    }
}