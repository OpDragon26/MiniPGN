namespace MiniPGN.Minimizer;

public static class Encoder
{
    
}

public readonly struct Version(int major, int minor)
{
    public override string ToString()
    {
        return $"v{major}.{minor}";
    }
}

public enum Type
{
    Fast,
    Overoptimized
}

public enum Metadata
{
    Exclude,
    Include
}