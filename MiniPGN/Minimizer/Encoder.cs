namespace MiniPGN.Minimizer;

public abstract class Encoder(Version version)
{
    protected readonly Version Version = version;

    public static readonly Encoder Active = new Standard.Standard(new Version(0,1));
    public abstract byte[] Encode(EncoderProfile profile, string fileName = "Result.mpgn");
}

public class EncoderProfile(Type type, Metadata metadataHandling, bool includeDate = false, bool includeGameCount = false)
{
    private readonly Type type = type;
    public readonly Metadata metadataHandling = metadataHandling;

    public readonly bool IncludeDate = includeDate;
    public readonly bool IncludeGameCount = includeGameCount;
    
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