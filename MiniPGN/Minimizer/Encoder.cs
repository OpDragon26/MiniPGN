namespace MiniPGN.Minimizer;

public abstract class Encoder(Version version)
{
    public readonly Version Version = version;

    public static Encoder Active = new Standard(new Version(0,1));
    public abstract byte[] Encode(EncoderProfile profile, string fileName = "Result.mpgn");
}

public class EncoderProfile(Type type, Metadata metadataHandling)
{
    public string[] file = [];

    public string FileMetadata()
    {
        return $"{type.ToString()[0]}{metadataHandling.ToString()[0]}";
    }
}

public readonly struct Version(int major, int minor)
{
    public override string ToString()
    {
        return $"v{major.ToString().PadLeft(2, '0')}.{minor.ToString().PadLeft(2, '0')}";
    }
}

public enum Type
{
    Fast,
    Standard,
    Overoptimized,
}

public enum Metadata
{
    Exclude,
    Include
}