using MiniPGN.Utils;

namespace MiniPGN.File_Handling.Compression;

public static class FileWriter
{
    public static void WriteToFile(string path, MPGNFile file)
    {
        File.WriteAllBytes(path, file.ToByteArray());
    }
    
    public static MPGNFile GenFileHeader(Config config)
    {
        MPGNFile file = new(config);
        
        // signature
        file.header.AddRange("MPGN".ToByteArray());
        
        // version
        file.header.AddRange(Info.Version.ToBytes());
        
        // encoding type
        file.header.Add((byte)(config.encoding == EncodingType.Standard ? 0x53 : 0x46));
        file.header.Add((byte)(config.metadataHandling == MetadataHandling.Include ? 0x49 : 0x45));
        
        // file metadata
        if (config.IncludeEncodingDate)
        {
            file.header.Add(0x01);
            file.header.AddRange(UnixDateBytes());
        }
        
        // game count metadata
        file.GameCountIndex = !config.IncludeGameCount ? -1 : (file.Count + 1); // needs to be changed once the number of games is actually known
        if (config.IncludeGameCount)
        {
            file.header.Add(0x02);
            file.header.AddRange(0UL.ToBytes());
        }
        
        file.header.Add(0xFF);
        
        return file;
    }

    static byte[] UnixDateBytes()
    {
        long time = DateTimeOffset.Now.ToUnixTimeSeconds();
        return ((ulong)time).ToBytes();
    }
}