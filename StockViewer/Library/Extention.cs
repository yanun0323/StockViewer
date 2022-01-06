
namespace StockViewer.Library;

public static class Extention
{
    private static readonly JsonSerializerOptions options = new JsonSerializerOptions() { };

    public static void SaveText(this string content, string filePath, string fileName)
        => SaveTextData(content, filePath, fileName);
    public static void SaveJson<T>(this T obj, string filePath, string fileName)
        => SaveJsonData(obj, filePath, fileName);

    public static string? LoadText(string filePath, string filename)
        => LoadTextData(filePath, filename);
    public static T? LoadJson<T>(string filePath, string fileName)
        => LoadJsonData<T>(filePath, fileName);

    private static T? LoadJsonData<T>(string path, string filename)
    {
        if (path.Length == 0 || filename.Length == 0)
            return default;

        if (!Directory.Exists(path))
            return default;
        try
        {
            using StreamReader streamReader = new(Path.Combine(path, filename), Encoding.Default);
            if (streamReader == null)
                return default;
            string? line = streamReader.ReadLine();
            streamReader.Close();
            if (line == null)
                return default;
            T? obj = JsonSerializer.Deserialize<T?>(line);
            return obj;
        }
        catch (FileNotFoundException)
        {
            return default;
        }
    }
    private static void SaveJsonData<T>(T obj, string path, string filename)
    {
        if (obj == null || path.Length == 0 || filename.Length == 0)
            return;

        if (!Directory.Exists(path))
            _ = Directory.CreateDirectory(path);

        using StreamWriter streamWriter = new(Path.Combine(path, filename), false, Encoding.Default);
        streamWriter.WriteLine(JsonSerializer.Serialize(obj, options));
        streamWriter.Flush();
        streamWriter.Close();
    }
    private static string? LoadTextData(string path, string filename)
    {
        if (path.Length == 0 || filename.Length == 0)
            return null;

        if (!Directory.Exists(path))
            return null;
        try
        {
            using StreamReader streamReader = new(Path.Combine(path, filename), Encoding.Default);
            if (streamReader == null)
                return null;
            string? line = streamReader.ReadLine();
            streamReader.Close();
            return line;
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }
    private static void SaveTextData(string content, string path, string filename)
    {
        if (content.Length == 0 || path.Length == 0 || filename.Length == 0)
            return;

        if (!Directory.Exists(path))
            _ = Directory.CreateDirectory(path);

        using StreamWriter streamWriter = new(Path.Combine(path, filename), false, Encoding.Default);
        streamWriter.WriteLine(content);
        streamWriter.Flush();
        streamWriter.Close();
    }
}
