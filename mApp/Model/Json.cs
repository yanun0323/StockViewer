namespace mApp.Model;
public static class Json
{
    public static T? LoadJsonData<T>(string dataPath, string filename)
    {
        try
        {
            StreamReader streamReader = new(Path.Combine(dataPath, filename));
            string? line = streamReader.ReadLine();
            if (line == null)
                return default;

            T obj = JsonSerializer.Deserialize<T>(line)!;

            streamReader.Close();
            return obj;
        }
        catch (Exception e)
        {
            Trace.WriteLine($"[ERROR] LoadJsonData: Can't find {filename} in {dataPath}");
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
                _ = Directory.CreateDirectory(dataPath!);

            if (!Directory.Exists(dataPath!))
                Trace.WriteLine($"[ERROR] SaveDatasAsJson: Can't create path for {obj.ToString}");


            StreamWriter streamWriter = new(Path.Combine(dataPath, filename));
            streamWriter.WriteLine(JsonSerializer.Serialize(obj));
            streamWriter.Flush();
            streamWriter.Close();
        }
        catch (Exception e)
        {
            Trace.WriteLine($"[ERROR] SaveDatasAsJson: {e}");
        }
    }
}
