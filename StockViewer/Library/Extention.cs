using System.Text.Json.Serialization;

namespace StockViewer.Library;
public static class FilePath
{
    public static string PathRoot { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StockViewer\\RawData"); }
    public static string NamePriceError = "PriceError";
    public static string NameInstitutionError = "InstitutionError";
    public static string NamePriceUpdateTime = "PriceUpdateTime";
    public static string NameInstitutionUpdateTime = "InstitutionUpdateTime";
    public static string PathPrice { get => Path.Combine(PathRoot, "Price"); }
    public static string PathInstitution { get => Path.Combine(PathRoot, "Institution"); }

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
