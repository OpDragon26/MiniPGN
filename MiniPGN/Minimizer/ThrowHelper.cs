namespace MiniPGN.Minimizer;

public static class ThrowHelper
{
    public static void StandardArgumentException()
    {
        throw new ArgumentException("Incorrect arguments. Correct use: 'mpgn -<encode/decode/v> <path> (the following only apply when encoding) -<fast/optimized> -<include/exclude> (metadata)'");
    }
}

public class ArgumentParsingException : Exception
{
    public ArgumentParsingException() { }
    public ArgumentParsingException(string message) : base(message) { }
    public ArgumentParsingException(string message, Exception inner) : base(message, inner) { }
}