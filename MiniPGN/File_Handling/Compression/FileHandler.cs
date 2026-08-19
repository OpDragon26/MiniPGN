namespace MiniPGN.File_Handling.Compression;

public static class FileHandler
{
    public static MPGNFile CompressFile(string path, Config config, bool log = false)
    {
        string[] file = File.ReadAllLines(path);
        MPGNFile result = FileWriter.GenFileHeader(config);
        FileParser.ParsePGNFile(file, result, log);
        return result;
    }
    
    static string[] ReadTextFile(string path)
    {
        return File.ReadAllLines(path);
    }
}