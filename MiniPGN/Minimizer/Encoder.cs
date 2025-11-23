namespace MiniPGN.Minimizer;

public static class Encoder
{
    public static Version Version = new(0,1);
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
    Overoptimized
}

public enum Metadata
{
    Exclude,
    Include
}