namespace MiniPGN.Minimizer;

public class Standard(Version version) : Encoder(version)
{
    public override byte[] Encode(EncoderProfile profile, string fileName = "Result.mpgn")
    {
        List<byte> ByteList = new();
        
        ByteList.AddString("MPGN");
        ByteList.AddString(Version.ToString());
        ByteList.AddString(profile.FileMetadata());
        
        return ByteList.ToArray();
    }
}