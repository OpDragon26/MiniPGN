using MiniPGN.Parsing;

namespace MiniPGN.Minimizer;

public static class DecodeHelper
{
    public static DecodeResult ExtractMetadata(IEnumerator<byte> file)
    {
        string signature = file.Extract(4).GetString();
        
        if (!signature.Equals("MPGN"))
            throw new MetadataExtractionException($"File signature incorrect. Expected 'MPGN' found {signature}");

        // mandatory metadata: version, type, and game metadata handling
        Version v = ExtractVersion(file);
        char type = file.Next().GetChar();
        char metaDataHandling = file.Next().GetChar();

        Type t = GetType(type);
        Metadata mh = GetMetadataHandling(metaDataHandling);
        
        TryExtractOptionalMetadata(file, out string date, out string gameCount);
        
        return new DecodeResult(new EncoderProfile(t, mh, !date.Equals(""), !gameCount.Equals("")), [], v, date, gameCount);
    }

    private static Version ExtractVersion(IEnumerator<byte> file)
    {
        string versionString = file
            .Extract(6)
            .GetString();
        
        if (versionString[0] != 'v' || versionString[3] != '.')
            throw new MetadataExtractionException($"Incorrect version format. Expected 'v##.##' found {versionString}'");

        try
        {
            int major = int.Parse(versionString[1..3]);
            int minor = int.Parse(versionString[4..6]);
            
            return new Version((byte)major, (byte)minor);
        }
        catch
        {
            throw new MetadataExtractionException($"Incorrect version format. Expected 'v##.##' found {versionString}'");
        }
    }
    
    private static Type GetType(char t)
    {
        return t switch
        {
            'S' => Type.Standard,
            'F' => Type.Fast,
            'O' => Type.Overoptimized,
            _ => throw new MetadataExtractionException($"Encoding type not recognized: {t}")
        };
    }

    private static Metadata GetMetadataHandling(char m)
    {
        return m switch
        {
            'I' => Metadata.Include,
            'E' => Metadata.Exclude,
            _ => throw new MetadataExtractionException($"Encoding type not recognized: {m}")
        };
    }

    private static void TryExtractOptionalMetadata(IEnumerator<byte> file, out string date, out string gameCount)
    {
        date = "";
        gameCount = "";
        
        byte tag = file.Next();

        while (tag != 0xFF)
        {
            switch (tag)
            {
                case 0x01: //date
                    date = ExtractDate(file);
                    break;
                case 0x02: // game count
                    gameCount = ExtractGameCount(file);
                    break;
            }
            
            tag = file.Next();
        }

        file.MoveNext();
        //Console.WriteLine(file.Current);
    }
    
    private static string ExtractDate(IEnumerator<byte> file)
    {
        byte[] bytes = file
            .Extract(7)
            .ToArray();

        int year = bytes[1] | (bytes[0] << 8);
        int month = bytes[2];
        int day = bytes[3];
        int hour = bytes[4];
        int minute = bytes[5];
        int second = bytes[6];
        
        return $"{year}.{month.Format()}.{day.Format()} {hour.Format()}:{minute.Format()}:{second.Format()}";
    }

    private static string ExtractGameCount(IEnumerator<byte> file)
    {
        IEnumerable<byte> bytes = file.Extract(8);

        return BitConverter.ToUInt64(bytes.Reverse().ToArray()).ToString();
    }

    private static string Format(this int n)
    {
        return n.ToString().PadLeft(2, '0');
    }
}