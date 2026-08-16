namespace MiniPGN.File_Handling;

public struct Config
{
    public MetadataHandling metadataHandling;
    public EncodingType encoding;
    public bool IncludeEncodingDate;
    public bool IncludeGameCount;
}

public enum MetadataHandling
{
    Include,
    Exclude
}

public enum EncodingType
{
    Standard,
    Fast
}