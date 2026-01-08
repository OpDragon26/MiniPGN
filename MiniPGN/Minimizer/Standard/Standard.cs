namespace MiniPGN.Minimizer.Standard;
using Parsing;
using Minimizer;

public class Standard(Version version) : Encoder(version)
{
    private static readonly Parser parser = new();
    
    public override byte[] Encode(EncoderProfile profile, string fileName = "Result.mpgn")
    {
        List<byte> byteList = new();
        ulong games = 0;
        
        foreach (string line in profile.file)
        {
            if (line.StartsWith('['))
            {
                if (profile.metadataHandling == Metadata.Include)
                    byteList.AddRange(TagPairParser.Parse(line));
            }
            else if (line.StartsWith('1'))
            {
                if (profile.metadataHandling == Metadata.Include)
                    byteList.Add(0xFF);
                byteList.AddRange(parser.ParseGame(line));
                
                if ((byteList[^1] & 0b11100111) == 11100111)
                    byteList.Add(0xFF);
                
                games++;
            }
        }
        
        List<byte> metaData = GetMetadata(profile, games);
        
        return metaData.Concat(byteList).ToArray();
    }

    private List<byte> GetMetadata(EncoderProfile profile, ulong numberOfGames = 0)
    {
        List<byte> byteList = new();
        
        // mandatory metadata
        byteList.AddRange("MPGN".ToByteArray(false));
        byteList.AddRange(Version.ToString().ToByteArray(false));
        byteList.AddRange(profile.FileMetadata().ToByteArray(false));
        
        // optional metaData
        if (profile.IncludeDate)
        {
            byteList.Add(0x01);
            
            DateTime date = DateTime.UtcNow;
            
            ushort year = (ushort)date.Year;
            byteList.AddRange(year.ToByteArray());
            byteList.Add((byte)date.Month);
            byteList.Add((byte)date.Day);
            
            byteList.Add((byte)date.Hour);
            byteList.Add((byte)date.Minute);
            byteList.Add((byte)date.Second);
        }

        if (profile.IncludeGameCount)
        {
            byteList.Add(0x02);
            byteList.AddRange(BitConverter.GetBytes(numberOfGames));
        }
        
        byteList.AddRange([0xFF, 0xFF]);
        
        return byteList;
    }
}