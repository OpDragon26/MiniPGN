using ChessLib.API.Display;
using ChessLib.API.Parsing;
using ChessLib.Base;
using MiniPGN.Utils;

namespace MiniPGN.Minimizer.Game;

public static class GameParser
{
    public static List<byte> ConvertGame(string pgn, bool log = false)
    {
        string[] tokens = pgn.Split(' ');
        IEnumerable<string> game = tokens.Where(ImportantToken);

        List<byte> bytes = [];
        Board board = Board.StartingBoard;
        bool checkMate = false;
        foreach (string token in game)
        {
            //Console.WriteLine(token);
            if (log)
                Console.WriteLine(token);
            if (token[^1] == ']')
                ParseEvalComment(token, bytes);
            else if (token.Contains('-') && !token.Contains('O'))
                ParseGameOverToken(token, bytes, checkMate);
            else
                ParseStandardMove(token, board, bytes, out checkMate, log);
        }
        
        return bytes;
    }

    private static void ParseStandardMove(string token, Board board, List<byte> bytes, out bool checkMate, bool log = false)
    {
        byte suffix = 0;
        if (TryParseMoveEvalSuffix(token, out byte s, out int l))
        {
            suffix = s;
            token = token[..^l];
        }

        if (log)
        {
            board.PrintBoard(debug: true);
            Console.WriteLine();
            Console.WriteLine(token);
        }
                
        Move move = board.ParseMove(token);
        byte[] moveCode = move.Convert(board);
        
        if (log)
            Console.WriteLine(moveCode.ToHexList());
        
        bytes.AddRange(moveCode);
        board.MakeMove(move);
        checkMate = token[^1] == '#';

        if (suffix != 0)
        {
            bytes.Add(0xF7);
            bytes.Add(suffix);
        }
    }

    private static bool TryParseMoveEvalSuffix(string token, out byte suffix, out int length)
    {
        length = 2;
        bool found = true;
        if (token.EndsWith("??"))
            suffix = 0x01;
        else if (token.EndsWith("?!"))
            suffix = 0x02;
        else if (token.EndsWith("!?"))
            suffix = 0x03;
        else if (token.EndsWith("!!"))
            suffix = 0x04;
        else if (token.EndsWith("?"))
        {
            suffix = 0x05;
            length = 1;
        }
        else if (token.EndsWith("!"))
        {
            suffix = 0x06;
            length = 1;
        }
        else
        {
            found = false;
            suffix = 0;
            length = 0;
        }
        
        return found;
    }

    private static void ParseGameOverToken(string token, List<byte> bytes, bool isCheckMate)
    {
        bytes.Add(token switch
        {
            "1-0" => (byte)(isCheckMate ? 0x2F : 0x0F),
            "0-1" => (byte)(isCheckMate ? 0x37 : 0x17),
            "1/2-1/2" => 0x07,
            _ => throw new ThrowHelper.MoveConverterException("unable to parse game over token: " + token)
        });
    }

    private static void ParseEvalComment(string token, List<byte> byteList)
    {
        if (token[0] == '#')
        {
            byteList.Add(0xEF);
            byteList.Add((byte)sbyte.Parse(token[1..^1]));
        }
        else
        {
            byteList.Add(0xE7);
            byteList.AddRange(float.Parse(token[..^1]).ToByteList());
        }
    }
    
    private static bool ImportantToken(string token)
    {
        return token[^1] != '.' 
               && token[0] != '['
               && token[0] != '{'
               && token[0] != '}';
    }
}