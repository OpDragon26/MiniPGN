using MiniPGN.Parsing;

namespace MiniPGN.Minimizer;

public static class DecodeHelper
{
    public static DecodeResult ExtractMetadata(IEnumerator<byte> file)
    {
        IEnumerable<byte> signature = file.Extract(4);
        Console.WriteLine(string.Join(' ', signature));
        
        throw new NotImplementedException();
    }
}