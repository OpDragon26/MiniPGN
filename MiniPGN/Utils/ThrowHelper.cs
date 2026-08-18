namespace MiniPGN.Utils;

public static class ThrowHelper
{
    [Serializable]
    public class MiniPGNException : Exception
    {
        public MiniPGNException () {}
        public MiniPGNException (string message) : base(message) {}
        public MiniPGNException (string message, Exception innerException) : base (message, innerException) {}    
    }
    
    [Serializable]
    public class InvalidTagException : MiniPGNException
    {
        public InvalidTagException () {}
        public InvalidTagException (string message) : base(message) {}
        public InvalidTagException (string message, Exception innerException) : base (message, innerException) {}    
    }
    
    [Serializable]
    public class MoveConverterException : MiniPGNException
    {
        public MoveConverterException () {}
        public MoveConverterException (string message) : base(message) {}
        public MoveConverterException (string message, Exception innerException) : base (message, innerException) {}    
    }
}