using System.Text.Json.Serialization;

namespace StockViewer.Library;
public static class FilePath
{
    public static string Path_Raw_Root { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StockViewer\\RawData"); }
    public static string Path_Raw_Price { get => Path.Combine(Path_Raw_Root, "Price"); }
    public static string Name_Error_Price = "Price_Error";
    public static string Name_UpdateTime_Price = "Price_UpdateTime";
    public static string Path_Raw_Institution { get => Path.Combine(Path_Raw_Root, "Institution"); }
    public static string Name_Error_Institution = "Institution_Error";
    public static string Name_UpdateTimeName_Institution = "Institution_UpdateTime";

}

public static class Extention
{
    private static readonly JsonSerializerOptions options = new JsonSerializerOptions() { };

    public static void SaveEachJson(this Queue<PriceCrawler> queue, string filePath)
    {
        while (queue.Any())
        {
            var item = queue.Dequeue();
            SaveDatasAsJson(item, Path.Combine(filePath, item.date.Remove(4)), item.date);
        }
    }
    public static void SaveEachJson(this Queue<InstitutionCrawler> queue, string filePath)
    {
        while (queue.Any())
        {
            var item = queue.Dequeue();
            SaveDatasAsJson(item, Path.Combine(filePath, item.date.Remove(4)), item.date);
        }
    }
    public static void SaveJson<T>(this T obj, string filePath, string fileName)
        => SaveDatasAsJson(obj, filePath, fileName);
    public static T? LoadJson<T>(string filePath, string fileName)
        => LoadJsonData<T>(filePath, fileName);


    private static T? LoadJsonData<T>(string dataPath, string filename)
    {
        if (dataPath.Length == 0 || filename.Length == 0)
            return default;

        if (!Directory.Exists(dataPath))
            return default;
        try
        {
            using StreamReader streamReader = new(Path.Combine(dataPath, filename));
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
    private static void SaveDatasAsJson<T>(T obj, string dataPath, string filename)
    {
        if (obj == null || dataPath.Length == 0 || filename.Length == 0)
            return;

        if (!Directory.Exists(dataPath))
            _ = Directory.CreateDirectory(dataPath);

        using StreamWriter streamWriter = new(Path.Combine(dataPath, filename));
        streamWriter.WriteLine(JsonSerializer.Serialize(obj, options));
        streamWriter.Flush();
        streamWriter.Close();
    }
}
