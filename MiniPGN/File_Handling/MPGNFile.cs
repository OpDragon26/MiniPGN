using MiniPGN.Utils;

namespace MiniPGN.File_Handling;

public class MPGNFile(Config config)
{
    public readonly List<byte> body = [];
    public readonly List<byte> header = [];
    public int GameCountIndex;
    public int Count => header.Count + body.Count;
    public string ToHexList => header.ToHexList() + " " + body.ToHexList();
    public readonly Config Config = config;
    
    public void AddGameCount(ulong gameCount)
    {
        if (GameCountIndex != -1)
        {
            byte[] gameCountBytes = gameCount.ToBytes();
            for (int i = 0; i < 8; i++)
                header[GameCountIndex + i] = gameCountBytes[i];
        }
    }

    public byte[] ToByteArray()
    {
        List<byte> result = new List<byte>(header.Count + body.Count);
        result.AddRange(header);
        result.AddRange(body);
        return result.ToArray();
    }
}