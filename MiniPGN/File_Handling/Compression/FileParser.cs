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
        
        foreach (string line in file)
        {
            //Console.WriteLine(line);
            if (line.StartsWith('[') && result.Config.metadataHandling == MetadataHandling.Include)
                tags.AddRange(TagParser.ParseTag(line));
            if (line.StartsWith('1'))
            {
                List<byte> game = GameParser.ConvertGame(line, log);
                if (!GameFinalized(game[^1]))
                    game.Add(0x3F);
                
                AddByteCountTag(result, (ushort)(tags.Count + game.Count + 4)); // +4: one for separation, 3 for the count itself 
                result.body.AddRange(tags);
                result.body.Add(0xFF);
                result.body.AddRange(game);
                
                gameCount++;
                tags = [];
            }
        }
        
        result.AddGameCount(gameCount);
    }

    static void AddByteCountTag(MPGNFile file, ushort count)
    {
        file.body.Add(0x01);
        file.body.AddRange(count.ToBytes());
    }

    private static readonly HashSet<byte> GameEndCodes = [0x07, 0x0F, 0x17, 0x2F, 0x37];
    static bool GameFinalized(byte b)
    {
        return GameEndCodes.Contains(b);
    }
}