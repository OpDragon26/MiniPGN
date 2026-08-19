using MiniPGN.Utils;

namespace MiniPGN.Minimizer.Metadata;

public static class OpeningDatabase
{
    public static readonly Dictionary<string, ushort> OpeningTable = new();
    public static readonly List<string> IndexTable = new();
    private static ushort Next;

    public static ushort Index(string opening)
    {
        if (OpeningTable.TryGetValue(opening, out ushort index))
            return index;
        IndexTable.Add(opening);
        ushort i = Next++;
        OpeningTable.Add(opening, i);
        return i;
    }
}