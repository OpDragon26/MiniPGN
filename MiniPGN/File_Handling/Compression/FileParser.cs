using MiniPGN.Minimizer;
using MiniPGN.Minimizer.Game;
using MiniPGN.Minimizer.Metadata;
using MiniPGN.Utils;

namespace MiniPGN.File_Handling.Compression;

public static class FileParser
{
    public static void ParsePGNFile(string[] file, MPGNFile result, bool log = false)
    {
        ulong gameCount = 0;
        List<byte> tags = [];
        GameData data = new();
        
        foreach (string line in file)
        {
            //Console.WriteLine(line);
            if (line.StartsWith('[') && result.Config.metadataHandling == MetadataHandling.Include)
                tags.AddRange(TagParser.ParseTag(line, data, tags.Count + 3));
            if (line.StartsWith('1'))
            {
                List<byte> game = GameParser.ConvertGame(line, log);
                if (!GameFinalized(game[^1]))
                    game.Add(0x3F);

                // byte count
                ushort count = (ushort)(tags.Count + game.Count + 4); // +4: one for separation, 3 for the count itself 
                data.ByteList.Add(0x01);
                data.ByteList.AddRange(count.ToBytes());
                
                data.ByteList.AddRange(tags);
                data.ByteList.Add(0xFF);
                data.ByteList.AddRange(game);
                
                gameCount++;
                tags = [];
                result.body.Add(data);
                data = new();
            }
        }
        
        NameIndex.IndexNames();
        GenOpeningIndex(result);
        GenPlayerIndex(result);
        result.AddGameCount(gameCount);
    }

    static void GenOpeningIndex(MPGNFile result)
    {
        foreach (string opening in OpeningDatabase.IndexTable)
            result.opening.AddRange(opening.ToByteArray(true));
        result.opening.Add(0xFF);
    }

    static void GenPlayerIndex(MPGNFile result)
    {
        foreach (string name in NameIndex.NameList)
            result.players.AddRange(name.ToByteArray(true));
        result.players.Add(0xFF);
    }
    
    static void AddByteCountTag(List<byte> bytes, ushort count)
    {
        bytes.Add(0x01);
        bytes.AddRange(count.ToBytes());
    }

    private static readonly HashSet<byte> GameEndCodes = [0x07, 0x0F, 0x17, 0x2F, 0x37];
    static bool GameFinalized(byte b)
    {
        return GameEndCodes.Contains(b);
    }
}