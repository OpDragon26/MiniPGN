namespace MiniPGN.Minimizer.Standard;
using Parsing;

public class Standard(Version version) : Encoder(version)
{
    public static Parser parser = new();
    
    public override byte[] Encode(EncoderProfile profile, string fileName = "Result.mpgn")
    {
        List<byte> ByteList = new();
        
        ByteList.AddRange("MPGN".ToByteArray(false));
        ByteList.AddRange(Version.ToString().ToByteArray(false));
        ByteList.AddRange(profile.FileMetadata().ToByteArray(false));

        foreach (string line in profile.file)
        {
            if (line.StartsWith('['))
            {
                if (profile.metadataHandling == Metadata.Include)
                    ByteList.AddRange(TagPairParser.Parse(line));
            }
            else if (line.StartsWith('1'))
            {
                if (profile.metadataHandling == Metadata.Include)
                    ByteList.Add(0xFF);
                ByteList.AddRange(parser.ParseGame(line));
            }
        }
        
        return ByteList.ToArray();
    }

    private void ParseGame(string line)
    {
        
    }
}