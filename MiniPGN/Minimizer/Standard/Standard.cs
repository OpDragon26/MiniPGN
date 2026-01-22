namespace MiniPGN.Minimizer.Standard;
using Parsing;
using Minimizer;

public class Standard(Version version) : EncodingHandler(version)
{
    private static readonly Encoder Encoder = new();
    
    public override byte[] Encode(EncoderProfile profile)
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
                byteList.AddRange(Encoder.ParseGame(line));
                
                if ((byteList[^1] & 0b11100111) != 0b11100111)
                    byteList.Add(0xFF);
                
                games++;
            }
        }
        
        List<byte> metaData = GetMetadata(profile, games);
        
        return metaData.Concat(byteList).ToArray();
    }

    public override DecodeResult Decode(byte[] bytes)
    {
        IEnumerator<byte> file = ((IEnumerable<byte>)bytes).GetEnumerator();
        file.MoveNext();

        DecodeResult result = DecodeHelper.ExtractMetadata(file);

        result.Result = result.Profile.metadataHandling == Metadata.Exclude
            ? ParseGamesWithoutMetadata(file)
            : ParseGamesWithMetadata(file);
        
        file.Dispose();
        
        Console.WriteLine(result);
        
        return result;
    }

    private IEnumerable<string> ParseGamesWithoutMetadata(IEnumerator<byte> file)
    {
        Decoder d = new(file);
        
        do
        {
            yield return d.ParseGame();
        } while (file.Current != 0xFF);
    }

    private IEnumerable<string> ParseGamesWithMetadata(IEnumerator<byte> file)
    {
        throw new NotImplementedException();
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
            byteList.AddRange(BitConverter.GetBytes(numberOfGames).Reverse());
        }
        
        byteList.AddRange([0xFF, 0xFF]);
        
        return byteList;
    }
}