using MiniPGN.Utils;

namespace MiniPGN.File_Handling;

public static class FileWriter
{
    public static MPGNFile GenFileHeader(Config config)
    {
        MPGNFile file = new();
        
        // signature
        file.AddRange("MPGN".ToByteArray());
        
        // encoding type
        file.Add((byte)(config.encoding == EncodingType.Standard ? 0x53 : 0x46));
        file.Add((byte)(config.metadataHandling == MetadataHandling.Include ? 0x49 : 0x45));
        
        // file metadata
        if (config.IncludeEncodingDate)
        {
            file.Add(0x01);
            file.AddRange(UnixDateBytes());
        }
        
        // game count metadata
        file.GameCountIndex = !config.IncludeGameCount ? -1 : file.Count; // needs to be changed once the number of games is actually known
        if (config.IncludeGameCount)
        {
            file.Add(0x02);
            file.AddRange(0UL.ToBytes());
        }
        
        return file;
    }

    public static byte[] UnixDateBytes()
    {
        long time = DateTimeOffset.Now.ToUnixTimeSeconds();
        return ((ulong)time).ToBytes();
    }
}