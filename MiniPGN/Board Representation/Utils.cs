namespace MiniPGN.Board_Representation;

public static class Utils
{
    public static int GetIndex(int file, int rank)
    {
        return rank * 8 + file;
    }

    public static ulong GetSquare(int index)
    {
        return 1UL << index;
    }
}