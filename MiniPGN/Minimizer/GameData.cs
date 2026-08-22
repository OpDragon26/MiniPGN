using MiniPGN.Minimizer.Metadata;
using MiniPGN.Utils;

namespace MiniPGN.Minimizer;

public class GameData
{
    public List<byte> ByteList = new();
    public string? WhiteName;
    public string? BlackName;
    public int WhiteInsertionPoint = -1;
    public int BlackInsertionPoint = -1;
    private bool namesInserted = false;

    public void SaveWhiteName(string name, int insertionPoint)
    {
        WhiteName = name;
        WhiteInsertionPoint = insertionPoint;
    }

    public void SaveBlackName(string name, int insertionPoint)
    {
        BlackName = name;
        BlackInsertionPoint = insertionPoint;
    }

    public void InsertNames()
    {
        if (namesInserted)
            return;
        namesInserted = true;
        
        if (BlackInsertionPoint != -1 && BlackName is not null)
            ByteList.InsertRange(BlackInsertionPoint, NameIndex.GetNameIndex(BlackName));
        if (WhiteInsertionPoint != -1 && WhiteName is not null)
            ByteList.InsertRange(WhiteInsertionPoint, NameIndex.GetNameIndex(WhiteName));
    }
}