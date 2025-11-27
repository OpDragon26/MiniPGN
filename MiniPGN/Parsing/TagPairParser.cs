namespace MiniPGN.Parsing;
using Minimizer;

public static class TagPairParser
{
    public static List<byte> Parse(string tagPair)
    {
        List<byte> bytes = new();
        
        string[] pair = tagPair[1..^1].Split(' ');
        pair[1] = string.Join(" ", pair[1..])[1..^1];
        
        switch (pair[0])
        {
            case "Event":
                bytes.Add(0x02);
                bytes.AddRange(ParseEventPair(pair[1]));
                break;
            
            case "Site":
                bytes.Add(0x03);
                bytes.AddRange(ParseSitePair(pair[1]));
                break;
            
            case "Round":
                bytes.Add(0x04);
                bytes.AddRange(ParseRound(pair[1]));
                break;
            
            case "White":
                bytes.Add(0x05);
                bytes.AddRange(tagPair.ToByteArray(true));
                break;
            
            case "Black":
                bytes.Add(0x06);
                bytes.AddRange(tagPair.ToByteArray(true));
                break;

            case "Result":
                bytes.Add(0x07);
                bytes.AddRange(ParseResult(pair[1]));
                break;

            case "Date":
                bytes.Add(0x08);
                bytes.AddRange(ParseDate(pair[1]));
                break;
            
            case "UTCDate":
                bytes.Add(0x09);
                bytes.AddRange(ParseDate(pair[1]));
                break;
            
            case "UTCTime":
                bytes.Add(0x0A);
                bytes.AddRange(ParseTime(pair[1]));
                break;
            
            case "TimeControl":
                bytes.Add(0x0B);
                bytes.AddRange(ParseTimeControl(pair[1]));
                break;
            
            case "WhiteElo":
                bytes.Add(0x0C);
                bytes.AddRange(ParseRating(pair[1]));
                break;
            
            case "BlackElo":
                bytes.Add(0x0D);
                bytes.AddRange(ParseRating(pair[1]));
                break;
            
            case "WhiteRatingDiff":
                bytes.Add(0x0E);
                bytes.AddRange(short.Parse(pair[1]).ToByteArray());
                break;
            
            case "BlackRatingDiff":
                bytes.Add(0x0F);
                bytes.AddRange(short.Parse(pair[1]).ToByteArray());
                break;
            
            case "ECO":
                bytes.Add(0x10);
                bytes.AddRange(ParseECOCode(pair[1]));
                break;
            
            case "Opening":
                bytes.Add(0x11);
                bytes.AddRange(pair[1].ToByteArray(true));
                break;
            
            case "Termination":
                bytes.Add(0x12);
                bytes.AddRange(ParseTermination(pair[1]));
                break;
            
            case "EndTime":
                bytes.Add(0x13);
                string[] time = pair[1].Split(' ');
                bytes.AddRange(ParseTime(time[0]));
                bytes.Add(ParseTimeZone(time[1]));
                break;
            
            case "Annotator":
                bytes.Add(0x14);
                bytes.AddRange(pair[1].ToByteArray(true));
                break;
            
            case "PlyCount":
                bytes.Add(0x15);
                bytes.AddRange(ushort.Parse(pair[1]).ToByteArray());
                break;
            
            case "Time":
                bytes.Add(0x16);
                bytes.AddRange(ParseTime(pair[1]));
                break;
            
            case "Mode":
                bytes.Add(0x17);
                bytes.AddRange(ParseMode(pair[1]));
                break;
            
            case "FEN":
                bytes.Add(0x18);
                bytes.AddRange(pair[1].ToByteArray(true));
                break;
            
            case "SetUp":
                bytes.Add(0x19);
                bytes.AddRange(ushort.Parse(pair[1]).ToByteArray());
                break;
            
            default:
                bytes.Add(0x01);
                bytes.AddRange(pair[0].ToByteArray(true));
                bytes.AddRange(pair[1].ToByteArray(true));
                break;
        }

        return bytes;
    }

    private static IEnumerable<byte> ParseMode(string tag)
    {
        if (tag.Equals("OTB"))
            yield return 0x02;
        if (tag.Equals("ICS"))
            yield return 0x03;
        else
        {
            yield return 0x01;
            foreach (byte b in tag.ToByteArray(true))
                yield return b;
        }
    }

    private static byte ParseTimeZone(string tag)
    {
        if (byte.TryParse(tag.Split('0')[1], out byte plusTime))
            return plusTime;
        throw new TagNotRecognizedException($"Unable to parse time zone {tag}");
    }
    
    private static IEnumerable<byte> ParseRound(string tag)
    {
        if (tag.Equals("?"))
        {
            yield return 0b1000_0000;
            yield return 0;
        }
        else if (ushort.TryParse(tag, out ushort rounds))
        {
            byte[] bytes = rounds.ToByteArray();
            yield return bytes[0];
            yield return bytes[1];
        }
        else
            throw new TagNotRecognizedException($"Could not parse round string {tag}");
    }
    
    private static readonly Dictionary<char, byte> ECOLetter = new()
    {
        {'A', 0},
        {'B', 2},
        {'C', 3},
        {'D', 4},
        {'E', 5},
    };

    private static IEnumerable<byte> ParseTermination(string tag)
    {
        switch (tag.ToLower())
        {
            case "normal":
                yield return 0x02;
                break;
            case "time forfeit":
                yield return 0x03;
                break;
            case "abandoned":
                yield return 0x04;
                break;
            case "adjudication":
                yield return 0x05;
                break;
            case "death":
                yield return 0x06;
                break;
            case "emergency":
                yield return 0x07;
                break;
            case "rules infraction":
                yield return 0x08;
                break;
            case "unterminated":
                yield return 0x09;
                break;
            
            default:
                yield return 0x01;
                foreach (byte b in tag.ToByteArray(true))
                    yield return b;
                break;
        }
    }
    
    private static IEnumerable<byte> ParseTimeControl(string tag)
    {
        if (tag.Contains('+'))
        {
            yield return 0x01;
            
            short[] time = tag.Split('+').Select(short.Parse).ToArray();
            
            foreach (byte b in time[0].ToByteArray())
                yield return b;
            foreach (byte b in time[1].ToByteArray())
                yield return b;
        }
        else if (short.TryParse(tag, out short time))
        {
            yield return 0x01;
            
            foreach (byte b in time.ToByteArray())
                yield return b;
            yield return 0;
            yield return 0;
        }
        else
        {
            yield return 0x02;

            foreach (byte b in tag.ToByteArray(true))
                yield return b;
        }
        
    }
    
    private static IEnumerable<byte> ParseECOCode(string tag)
    {
        yield return ECOLetter[tag[0]];
        yield return byte.Parse(tag[1..]);
    }
    
    private static IEnumerable<byte> ParseRating(string tag)
    {
        ushort elo = ushort.Parse(tag);
        yield return (byte)(elo >> 8);
        yield return (byte)elo;
    }

    private static IEnumerable<byte> ParseTime(string tag)
    {
        byte[] time = tag.Split(':').Select(byte.Parse).ToArray();
        yield return time[0];
        yield return time[1];
        yield return time[2];
    }
    
    private static IEnumerable<byte> ParseDate(string tag)
    {
        ushort[] date = tag.Split('.').Select(ushort.Parse).ToArray();
        yield return (byte)(date[0] >> 8);
        yield return (byte)date[0];
        yield return (byte)date[1];
        yield return (byte)date[2];
    }

    private static IEnumerable<byte> ParseResult(string tag)
    {
        yield return tag switch
        {
            "1-0" => 0x01, // white won
            "0-1" => 0x02, // black wom
            "1/2-1/2" => 0x03, // draw
            _ => throw new TagNotRecognizedException($"Result not recognized: {tag}")
        };
    }
    
    private static IEnumerable<byte> ParseSitePair(string tag)
    {
        if (tag.Equals("Chess.com"))
            yield return 0x02;
        
        else if (tag.StartsWith("https://lichess.org/"))
        {
            yield return 0x03;

            foreach (byte b in tag[20..].ToByteArray(true))
                yield return b;
            
            yield break;
        }
        
        yield return 0x01;
        foreach (byte b in tag.ToByteArray(true))
            yield return b;
    }

    private static IEnumerable<byte> ParseEventPair(string tag)
    {
        // chess.com game
        if (tag.Equals("Live Chess"))
            yield return 0x02;
                
        // Lichess rated
        else if (tag.StartsWith("Rated"))
        {
            yield return 0x03;
            
            string timeControl = tag.Split(' ')[1];
            yield return timeControl switch
            {
                "Bullet" => 0x01,
                "Blitz" => 0x02,
                "Classical" => 0x03,
                "Correspondence" => 0x04,
                _ => throw new TagNotRecognizedException($"Could not recognize Lichess time control: {timeControl}")
            };
            
            yield break;
        }

        yield return 0x01;
        foreach (byte b in tag.ToByteArray(true))
            yield return b;
    }
}