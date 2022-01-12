
using System.Buffers;
using System.Buffers.Text;

namespace StockViewer.Library;

public static class FileManagement
{
    private static readonly JsonSerializerOptions options = new JsonSerializerOptions() { Converters = { new LongToStringJsonConverter() } , AllowTrailingCommas = true};
    public static object Key = new();
    public static void SaveText(this string content, string path, string name)
        => SaveTextData(content, path, name);
    public static void SaveJson<T>(this T obj, string path, string name)
        => SaveJsonData(obj, path, name);

    public static string? LoadText(string path, string name)
        => LoadTextData(path, name);
    public static T? LoadJson<T>(string path, string name, bool convert = true)
        => LoadJsonData<T>(path, name, convert);

    private static T? LoadJsonData<T>(string path, string filename, bool convert)
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
            string? line = streamReader.ReadToEnd();
            streamReader.Close();
            if (line == null)
                return default;
            var readOnlySpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(line));
            T? obj = JsonSerializer.Deserialize<T?>(readOnlySpan, convert ? options : null);
            return obj;
        }
        catch (FileNotFoundException)
        {
            return default;
        }
    }
    private static void SaveJsonData<T>(T obj, string path, string filename)
    {
        lock (Key)
        {
            if (obj == null || path.Length == 0 || filename.Length == 0)
                return;

            if (!Directory.Exists(path))
                _ = Directory.CreateDirectory(path);

            using StreamWriter streamWriter = new(Path.Combine(path, filename), false, Encoding.Default);
            streamWriter.WriteLine(JsonSerializer.Serialize(obj));
            streamWriter.Flush();
            streamWriter.Close();
        }
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
            string? line = streamReader.ReadToEnd();
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

    private class LongToStringJsonConverter : JsonConverter<string>
    {
        public LongToStringJsonConverter() { }

        public override string Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number &&
                type == typeof(String))
                return reader.GetString() ?? "";

            var span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;

            if (Utf8Parser.TryParse(span, out long number, out var bytesConsumed) && span.Length == bytesConsumed)
                return number.ToString();

            var data = reader.GetString();

            throw new InvalidOperationException($"'{data}' is not a correct expected value!")
            {
                Source = "LongToStringJsonConverter"
            };
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
