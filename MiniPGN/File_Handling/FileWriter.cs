using MiniPGN.Utils;

namespace MiniPGN.File_Handling;

public static class FileWriter
{
    public static List<byte> GenFileHeader(Config config)
    {
        List<byte> bytes = [];
        
        // signature
        bytes.AddRange("MPGN".ToByteArray());
        
        // encoding type
        bytes.Add((byte)(config.encoding == EncodingType.Standard ? 0x53 : 0x46));
        bytes.Add((byte)(config.metadataHandling == MetadataHandling.Include ? 0x49 : 0x45));
        
        // file metadata
        if (config.IncludeEncodingDate)
        {
            bytes.Add(0x01);
            bytes.AddRange(UnixDateBytes());
        }
        
        // game count metadata
        int GameCountIndex = !config.IncludeGameCount ? -1 : bytes.Count; // needs to be changed once the number of games is actually known
        if (config.IncludeGameCount)
        {
            bytes.Add(0x02);
            bytes.AddRange(0UL.ToBytes());
        }
        
        
        
        return bytes;
    }

    public static byte[] UnixDateBytes()
    {
        long time = DateTimeOffset.Now.ToUnixTimeSeconds();
        return ((ulong)time).ToBytes();
    }
}