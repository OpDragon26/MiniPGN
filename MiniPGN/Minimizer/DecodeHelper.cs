using MiniPGN.Parsing;

namespace MiniPGN.Minimizer;

public static class DecodeHelper
{
    public static DecodeResult ExtractMetadata(IEnumerator<byte> file)
    {
        string signature = file.Extract(4).GetString();
        
        if (!signature.Equals("MPGN"))
            throw new MetadataExtractionException($"File signature incorrect. Expected 'MPGN' found {signature}");

        // mandatory metadata: type and game metadata handling
        char type = file.Next().GetChar();
        char metaDataHandling = file.Next().GetChar();

        Type t = GetType(type);
        Metadata mh = GetMetadataHandling(metaDataHandling);
        
        TryExtractOptionalMetadata(file, out string date, out string gameCount);
        
        return new DecodeResult(new EncoderProfile(t, mh, date == "", gameCount == ""), [], date, gameCount);
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
        
        return $"{year}.{month}.{day} {hour}:{minute}:{second}";
    }

    private static string ExtractGameCount(IEnumerator<byte> file)
    {
        byte[] bytes = file
            .Extract(8)
            .ToArray();

        return BitConverter.ToUInt64(bytes).ToString();
    }
}