namespace MiniPGN.Parsing;

public static class TagPairDecoder
{
    public static IEnumerable<string> ParseTags(IEnumerator<byte> file)
    {
        while (file.Current != 0xFF)
        {
            byte tag = file.Next();
            
            
        }
        
        yield break;
    }
}