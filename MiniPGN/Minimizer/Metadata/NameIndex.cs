using MiniPGN.Utils;

namespace MiniPGN.Minimizer.Metadata;

public static class NameIndex
{
    private static readonly Counter<string> Index = new();

    public static void Add(string name)
    {
        Index.AddCount(name);
    }
    
    public static byte[] GetNameIndex(string name)
    {
        return [0xAF, 0xFA];
    }
}