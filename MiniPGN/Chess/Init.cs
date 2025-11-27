namespace MiniPGN.Chess;

public static class Init
{
    private static bool complete;
    
    public static void Start()
    {
        if (complete)
            return;
        complete = true;
        
        Bitboards.Masks.Init();
    }
}