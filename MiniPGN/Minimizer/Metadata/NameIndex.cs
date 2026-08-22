using MiniPGN.Utils;

namespace MiniPGN.Minimizer.Metadata;

public static class NameIndex
{
    private static readonly Counter<string> NameCounter = new();

    public static void Add(string name)
    {
        //Console.WriteLine(name);
        NameCounter.AddCount(name);
    }
    
    public static byte[] GetNameIndex(string name)
    {
        uint index = Indexer[name];

        if (index <= 127)
            return [(byte)index];
        if (index <= 16383)
            return ((ushort)(index | 0x8000)).ToBytes();
        return (index | 0xC0000000).ToBytes();
    }

    private static Dictionary<string, uint> Indexer = new();
    public static List<string> NameList = new();

    public static void IndexNames()
    {
        Indexer = new();
        uint index = 0;
        
        Indexer.Clear();
        NameList.Clear();

        foreach (string name in NameCounter.GetSorted().Reverse().Select(x => x.Key))
        {
            //Console.WriteLine($"{name} - {index}");
            Indexer.Add(name, index++);
            NameList.Add(name);
        }
    }
}