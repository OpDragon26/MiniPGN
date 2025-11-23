namespace MiniPGN.Minimizer;

public static class FileReader
{
    public static EncoderProfile GetProfile(string[] args)
    {
        EncoderProfile profile = new();
        
        if (args.Length > 1)
            ThrowHelper.StandardArgumentException();

        if (Version.Contains(args[0]))
        {
            Console.WriteLine($"Current version: {Encoder.Version}");
            return profile;
        }

        if (Encode.Contains(args[0]))
        {
            profile.action = ActionType.Encode;
            
            if (args.Length > 4)
                ThrowHelper.StandardArgumentException();
            
            if (!CorrectEncodePath(args[1]))
                throw new ArgumentException("File not found or is not a .pgn or a .txt file");
            
            profile.path = args[1];
        }
        else if (Decode.Contains(args[0]))
        {
            if (args.Length > 2)
                ThrowHelper.StandardArgumentException();
            
            if (!CorrectDecodePath(args[1]))
                throw new ArgumentException("File not found or is not a .pgn or a .txt file");
            
            profile.path = args[1];
            
            TryReadFileMetadata(profile);
        }
        
        ThrowHelper.StandardArgumentException();
        return profile;
    }

    private static readonly string[] Encode = ["-e", "--e", "-encode", "--encode"];
    private static readonly string[] Decode = ["-d", "--d", "-decode", "--decode"];
    private static readonly string[] Version = ["-v", "--v", "-version", "--version"];
    
    private static bool CorrectEncodePath(string path)
    {
        return File.Exists(path) && (path.EndsWith(".pgn") || path.EndsWith(".txt"));
    }
    
    private static bool CorrectDecodePath(string path)
    {
        return File.Exists(path) && path.EndsWith(".mpgn");
    }

    private static void TryReadFileMetadata(EncoderProfile profile)
    {
        profile.file = File.ReadAllBytes(profile.path);

        if (profile.file.Length < 12)
            throw new ArgumentParsingException("Invalid file signature");

        byte[] signature = profile.file[..4];
        Console.WriteLine(string.Join(" ", signature));
    }
}

public class EncoderProfile
{
    public ActionType action;
    public string path = "";
    
    public Version version;
    public Type type;
    public Metadata metadataHandling;

    public byte[] file;
}

public enum ActionType
{
    None,
    Encode,
    Decode
}
