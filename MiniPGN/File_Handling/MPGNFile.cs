using MiniPGN.Utils;

namespace MiniPGN.File_Handling;

public class MPGNFile(Config config)
{
    public readonly List<byte> header = [];
    public readonly List<byte> opening = [];
    public readonly List<byte> body = [];
    
    public int GameCountIndex;
    public int Count => header.Count + opening.Count + body.Count;
    public string ToHexList => header.ToHexList() + " " + opening.ToHexList() + " " + body.ToHexList();
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
        List<byte> result = new List<byte>(Count);
        result.AddRange(header);
        result.AddRange(opening);
        result.AddRange(body);
        return result.ToArray();
    }
}