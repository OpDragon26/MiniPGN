using MiniPGN.Minimizer;

namespace MiniPGN.Parsing;

public static class TagPairDecoder
{
    public static IEnumerable<string> ParseTags(IEnumerator<byte> file)
    {
        while (file.Current != 0xFF)
        {
            byte tag = file.Next();

            yield return tag switch
            {
                0x01 => ParseGenericTag(file),
                0x02 => ParseEventTag(file),
                0x03 => ParseSiteTag(file),
                0x04 => ParseRoundTag(file),
                0x05 => ParseNameTag(file, "White"),
                0x06 => ParseNameTag(file, "Black"),
                0x07 => ParseResultTag(file),
                0x08 => ParseDateTag(file, false),
                0x09 => ParseDateTag(file, true),
                0x0A => ParseTimeTag(file),
                0x0B => ParseTimeControlTag(file),
                _ => throw new TagNotRecognizedException($"Unknown tag: {tag}")
            };
        }
    }

    private static string ParseTimeControlTag(IEnumerator<byte> file)
    {
        byte[] bytes = file.Extract(4).ToArray();
        
        int time = bytes[1] | (bytes[0] << 8);
        int bonus = bytes[3] | (bytes[2] << 8);

        if (bonus == 0xF0F0)
            return GenTagPair("TimeControl", time.ToString());
        return GenTagPair("TimeControl", $"{time}+{bonus}");
    }

    private static string ParseTimeTag(IEnumerator<byte> file)
    {
        byte[] bytes = file.Extract(3).ToArray();
        
        string hour = bytes[0].ToString().PadLeft(2, '0');
        string minute = bytes[1].ToString().PadLeft(2, '0');
        string second = bytes[2].ToString().PadLeft(2, '0');
        
        string timeStr = hour + ":" + minute + ":" + second;
        
        return GenTagPair("UTCTime", timeStr);
    }
    
    private static string ParseDateTag(IEnumerator<byte> file, bool lichess)
    {
        string date = GetDateValue(file);
        return GenTagPair(lichess ? "UTCDate" : "Date", date);
    }

    private static string GetDateValue(IEnumerator<byte> file)
    {
        byte[] bytes = file.Extract(4).ToArray();

        int year = bytes[1] | (bytes[0] << 8);
        int month = bytes[2];
        int day = bytes[3];
        
        return $"{year}.{month.ToString().PadLeft(2, '0')}.{day.ToString().PadLeft(2, '0')}";
    }

    private static string ParseResultTag(IEnumerator<byte> file)
    {
        byte tag = file.Next();

        string result = tag switch
        {
            0x01 => "1-0",
            0x02 => "0-1",
            0x03 => "1/2-1/2",
            _ => throw new TagNotRecognizedException($"Unknown result tag: {tag}")
        };
        
        return GenTagPair("Result", result);
    }
    
    private static string ParseNameTag(IEnumerator<byte> file, string color)
    {
        string value = file.ExtractNullTerminatedString();
        return GenTagPair(color, value);
    }
    
    private static string ParseRoundTag(IEnumerator<byte> file)
    {
        byte[] bytes = file.Extract(2).ToArray();
        if ((bytes[0] & 0b1000_0000) == 0)
            return GenTagPair("Round", "?");
        int round = bytes[1] | (bytes[0] << 8);
        return GenTagPair("Round", round.ToString());
    }

    private static string ParseSiteTag(IEnumerator<byte> file)
    {
        byte tag = file.Next();

        switch (tag)
        {
            case 0x01:
                string value = file.ExtractNullTerminatedString();
                return GenTagPair("Site", value);
            
            case 0x02:
                return GenTagPair("Site", "Chess.com");
            
            case 0x03:
                string link = file.ExtractNullTerminatedString();
                return GenTagPair("Site", $"https://lichess.org/{link}");
        }

        throw new TagNotRecognizedException($"Unknow site tag: {tag.GetBinaryString()}");
    }
    
    private static readonly string[] TimeControls = ["Bullet", "Blitz", "Classical", "Correspondence"];
    private static string ParseEventTag(IEnumerator<byte> file)
    {
        byte tag = file.Next();
        
        switch (tag)
        {
            case 0x01: // not recognized
                string value = file.ExtractNullTerminatedString();
                return GenTagPair("Event", value);
            
            case 0x02: // chess.com
                return GenTagPair("Event", "Live Chess");
            
            case 0x03: // lichess
                byte tc = file.Next();
                string timeControl = TimeControls[tc - 1];
                return GenTagPair("Event", $"Rated {timeControl} game");
        }

        throw new TagNotRecognizedException($"Unknown event tag: {tag.GetBinaryString()}");
    }

    private static string ParseGenericTag(IEnumerator<byte> file)
    {
        string name = file.ExtractNullTerminatedString();
        string value = file.ExtractNullTerminatedString();
        return GenTagPair(name, value);
    }

    private static string GenTagPair(string name, string value)
    {
        return $"[{name} \"{value}\"]";
    }
}