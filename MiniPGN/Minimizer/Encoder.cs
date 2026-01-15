using System.Diagnostics.CodeAnalysis;

namespace MiniPGN.Minimizer;

public abstract class Encoder(Version version)
{
    protected readonly Version Version = version;

    public static readonly Encoder Active = new Standard.Standard(new Version(0,1));
    public abstract byte[] Encode(EncoderProfile profile, string fileName = "Result.mpgn");
    public abstract DecodeResult Decode(byte[] file);
}

public class EncoderProfile(Type type, Metadata metadataHandling, bool includeDate = false, bool includeGameCount = false)
{
    public readonly Type type = type;
    public readonly Metadata metadataHandling = metadataHandling;

    public readonly bool IncludeDate = includeDate;
    public readonly bool IncludeGameCount = includeGameCount;
    
    public string[] file = [];

    public string FileMetadata()
    {
        return $"{type.ToString()[0]}{metadataHandling.ToString()[0]}";
    }
}

public readonly struct Version(byte major, byte minor) : IEquatable<Version>
{
    private readonly byte Major = major;
    private readonly byte Minor = minor;
    
    public override string ToString()
    {
        return $"v{Major.ToString().PadLeft(2, '0')}.{Minor.ToString().PadLeft(2, '0')}";
    }

    public bool Equals(Version other)
    {
        return Major == other.Major && Minor == other.Minor;
    }
}

public class DecodeResult(EncoderProfile profile, IEnumerable<string> result, Version version, string encodeDate = "", string gameCount = "")
{
    private readonly EncoderProfile Profile = profile;
    private readonly IEnumerable<string> Result = result;
    private readonly string EncodeDate = encodeDate;
    private readonly string GameCount = gameCount;
    private readonly Version Version = version;

    public override string ToString()
    {
        return $"Decode Result:\n" +
               $"----------------------------------------------------\n" +
               $"| Version: {Version}\n" +
               $"| Encoding: {Profile.type}\n" +
               $"| Metadata: {Profile.metadataHandling}\n" +
               $"{(Profile.IncludeDate ? $"| Date (GMT): {EncodeDate}\n" : "")}" +
               $"{(Profile.IncludeGameCount ? $"| Number of games: {GameCount}\n" : "")}" +
               $"| Result: {Result.Count()} lines";
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