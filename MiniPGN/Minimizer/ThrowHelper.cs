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

public class MetadataExtractionException : MiniPGNException
{
    public MetadataExtractionException() { }
    public MetadataExtractionException(string message) : base(message) { }
    public MetadataExtractionException(string message, Exception inner) : base(message, inner) { }
}

public class MoveDecodingException : MiniPGNException
{
    public MoveDecodingException() { }
    public MoveDecodingException(string message) : base(message) { }
    public MoveDecodingException(string message, Exception inner) : base(message, inner) { }
}