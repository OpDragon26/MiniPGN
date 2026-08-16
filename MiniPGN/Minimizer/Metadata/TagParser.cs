using System.Text.RegularExpressions;
using MiniPGN.Utils;

namespace MiniPGN.Minimizer.Metadata;

public static class TagParser
{
    public static List<byte> ParseTag(string line)
    {
        List<byte> bytes = [];
        
        string[] pair = SplitTag(line);
        string tag = pair[0];
        string data = pair[1];

        bytes.AddRange(tag switch
        {
            "Event" => GetEventTagBytes(data),
            "Site" => GetSiteTagBytes(data),
            "Round" => GetRoundTagBytes(data),
            "White" => GetTextTagBytes(0x05, data),
            "Black" => GetTextTagBytes(0x06, data),
            "Result" => GetResultTagBytes(data),
            "Date" => GetDateTagBytes(tag, data),
            "UTCDate" => GetDateTagBytes(tag, data),
            "UTCTime" => GetTimeTagBytes(0x0A, data),
            "TimeControl" => GetTimeControlTagBytes(data),
            "WhiteElo" => GetUshortTagBytes(0x0C, data),
            "BlackElo" => GetUshortTagBytes(0x0D, data),
            "WhiteRatingDiff" => GetEloDiffTagBytes(tag, data),
            "BlackRatingDiff" => GetEloDiffTagBytes(tag, data),
            "ECO" => GetEcoCodeTagBytes(data),
            "Opening" => GetTextTagBytes(0x11, data),
            "Termination" => GetTerminationTagBytes(data),
            "EndTime" => GetEndTimeTagBytes(data),
            "Annotator" => GetTextTagBytes(0x14, data),
            "PlyCount" => GetUshortTagBytes(0x15, data),
            "Time" => GetTimeTagBytes(0x16, data),
            "Mode" => GetModeTagBytes(data),
            "FEN" => GetTextTagBytes(0x18, data),
            _ => GetUnknownTagBytes(tag, data)
        });
        
        return bytes;
    }

    static IEnumerable<byte> GetModeTagBytes(string data)
    {
        yield return 0x17;
        if (data.Equals("OTB"))
            yield return 0x02;
        else if (data.Equals("ICS"))
            yield return 0x03;
        else
        {
            yield return 0x01;
            foreach (var b in data.ToByteArray(true))
                yield return b;
        }
            
    }

    static IEnumerable<byte> GetEndTimeTagBytes(string data)
    {
        yield return 0x13;
        string[] split = data.Split();
        string[] time = split[0].Split(':');
        byte[] gmt = ((ushort)short.Parse(split[1][3..])).ToBytes();
        
        yield return byte.Parse(time[0]);
        yield return byte.Parse(time[1]);
        yield return byte.Parse(time[2]);
        yield return gmt[0];
        yield return gmt[1];
    }

    static IEnumerable<byte> GetTerminationTagBytes(string data)
    {
        yield return 0x12;

        switch (data)
        {
            case "Normal": yield return 0x02; break;
            case "Time forfeit": yield return 0x03; break;
            case "Abandoned": yield return 0x04; break;
            case "Adjudication": yield return 0x05; break;
            case "Death": yield return 0x06; break;
            case "Emergency": yield return 0x07; break;
            case "Rules infraction": yield return 0x08; break;
            case "Unterminated": yield return 0x09; break;
            default:
                yield return 0x01;
                foreach (var b in data.ToByteArray(true))
                    yield return b;
                break;
        }
    }

    static IEnumerable<byte> GetEcoCodeTagBytes(string data)
    {
        yield return 0x10;
        yield return (byte)data[0];
        yield return byte.Parse(data[1..]);
    }

    static IEnumerable<byte> GetEloDiffTagBytes(string tag, string data)
    {
        yield return (byte)(tag[0] == 'W' ? 0x0E : 0x0F);

        byte[] elo = ((ushort)short.Parse(data)).ToBytes();
        yield return elo[0];
        yield return elo[1];
    }
    
    static IEnumerable<byte> GetUshortTagBytes(byte tag, string data)
    {
        yield return tag;
        byte[] elo = ushort.Parse(data).ToBytes();
        yield return elo[0];
        yield return elo[1];
    }

    static IEnumerable<byte> GetTimeControlTagBytes(string data)
    {
        yield return 0x0B;

        if (data.Contains('+'))
        {
            string[] timeControl = data.Split('+');
            byte[] time = ushort.Parse(timeControl[0]).ToBytes();
            byte[] bonus = ushort.Parse(timeControl[1]).ToBytes();
            
            yield return 0x02;
            yield return time[0];
            yield return time[1];
            yield return bonus[0];
            yield return bonus[1];
        }
        else if (ushort.TryParse(data, out ushort v))
        {
            byte[] time = v.ToBytes();
            
            yield return 0x02;
            yield return time[0];
            yield return time[1];
        }
        else
        {
            yield return 0x01;
            foreach (var b in data.ToByteArray(true))
                yield return b;
        }
    }
    
    static IEnumerable<byte> GetTimeTagBytes(byte tag, string data)
    {
        yield return tag;
        
        string[] time = data.Split(':');
        yield return byte.Parse(time[0]);
        yield return byte.Parse(time[1]);
        yield return byte.Parse(time[2]);
    }
    
    static IEnumerable<byte> GetDateTagBytes(string tag, string data)
    {
        yield return (byte)(tag[0] == 'U' ? 0x09 : 0x08);
        
        string[] date = data.Split('.');
        byte[] year = ushort.Parse(date[0]).ToBytes();
        yield return year[0];
        yield return year[1];
        yield return byte.Parse(date[1]);
        yield return byte.Parse(date[2]);
    }
    
    static IEnumerable<byte> GetResultTagBytes(string data)
    {
        yield return 0x07;
        yield return data switch
        {
            "1-0" => 0x01,
            "0-1" => 0x02,
            "1/2-1/2" => 0x03,
            _ => throw new ThrowHelper.InvalidTagException("Invalid result data: " + data)
        };
    }

    static IEnumerable<byte> GetTextTagBytes(byte tag, string data)
    {
        yield return tag;
        foreach (var b in data.ToByteArray(true))
            yield return b;
    }
    
    static IEnumerable<byte> GetRoundTagBytes(string data)
    {
        yield return 0x04;
        if (data.Equals("?"))
            yield return 0x03;
        else if (byte.TryParse(data, out byte result))
        {
            yield return 0x02;
            yield return result;
        }
        else
        {
            yield return 0x01;
            foreach (var b in data.ToByteArray(true))
                yield return b;
        }
    }
    
    static IEnumerable<byte> GetSiteTagBytes(string data)
    {
        yield return 0x03;
        if (data.Equals("Chess.com"))
            yield return 0x02;
        else if (data.StartsWith("https://lichess.org/"))
        {
            yield return 0x03;
            foreach (var b in data[20..].ToByteArray(true))
                yield return b;
        }
        else
        {
            yield return 0x01;
            foreach (var b in data.ToByteArray(true))
                yield return b;
        }
    }
    
    static IEnumerable<byte> GetEventTagBytes(string data)
    {
        yield return 0x02;
        if (data.Equals("Live Chess"))
            yield return 0x02;
        else if (data.StartsWith("Rated ") && data.EndsWith(" game"))
        {
            yield return 0x03;
            yield return data.Split()[1] switch
            {
                "Bullet" => 0x01,
                "Blitz" => 0x02,
                "Classical" => 0x03,
                "Correspondence" => 0x04,
                _ => throw new ThrowHelper.InvalidTagException("Invalid result data: " + data)
            };
        }
        else
        {
            yield return 0x01;
            foreach (var b in data.ToByteArray(true))
                yield return b;
        }
    }

    static IEnumerable<byte> GetUnknownTagBytes(string tag, string data)
    {
        yield return 0xFE;
        foreach (var b in tag.ToByteArray(true))
            yield return b;
        foreach (var b in data.ToByteArray(true))
            yield return b;
    }

    private static readonly Regex regex = new(@"[[""\]]", RegexOptions.Compiled);
    static string[] SplitTag(string tag)
    {
        tag = regex.Replace(tag,""); // removes '[', ']' and '"'
        return tag.Split(' ', 2);
    }
}