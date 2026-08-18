namespace MiniPGN.File_Handling;

public class MPGNFile
{
    public readonly List<byte> file = [];
    public int GameCountIndex;

    public int Count => file.Count;
    
    public void Add(byte b)
    {
        file.Add(b);
    }
    
    public void AddRange(IEnumerable<byte> bytes)
    {
        file.AddRange(bytes);
    }
}