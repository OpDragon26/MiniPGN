namespace MiniPGN.Minimizer;

public static class ThrowHelper
{

}

// never thrown, only inherited by other exceptions
public class MiniPGNException : Exception
{
    protected MiniPGNException() { }
    protected MiniPGNException(string message) : base(message) { }
    protected MiniPGNException(string message, Exception inner) : base(message, inner) { }
}

public class TagNotRecognizedException : MiniPGNException
{
    public TagNotRecognizedException() { }
    public TagNotRecognizedException(string message) : base(message) { }
    public TagNotRecognizedException(string message, Exception inner) : base(message, inner) { }
}

public class NotationParsingException : MiniPGNException
{
    public NotationParsingException() { }
    public NotationParsingException(string message) : base(message) { }
    public NotationParsingException(string message, Exception inner) : base(message, inner) { }
}