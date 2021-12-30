using System.Text.Json.Serialization;

namespace StockViewer.Library;
public static class Json
{
    public static T? LoadJsonData<T>(string dataPath, string filename)
    {
        try
        {
            using StreamReader? streamReader = new(Path.Combine(dataPath, filename));
            if (streamReader == null)
            {
                streamReader?.Close();
                return default;
            }

            string? line = streamReader.ReadLine();
            streamReader.Close();

            if (line == null)
                return default;

            T obj = JsonSerializer.Deserialize<T>(line)!;

            return obj;
        }
        catch (Exception e)
        {
            Trace.WriteLine($"[ERROR] LoadJsonData: {e}");
        }
        return default;
    }
    public static void SaveDatasAsJson<T>(T obj, string dataPath, string filename)
    {
        if (obj == null || dataPath.Length == 0 || filename.Length == 0)
            return;
        try
        {
            if (!Directory.Exists(dataPath!))
                _ = Directory.CreateDirectory(dataPath);

            using StreamWriter? streamWriter = new(Path.Combine(dataPath, filename));
            var result = JsonSerializer.Serialize(obj);
            streamWriter.WriteLine(result);
            streamWriter.Flush();
            streamWriter.Close();
        }
        catch (Exception e)
        {
            Trace.WriteLine($"[ERROR] SaveDatasAsJson: {e}");
        }
    }
}
