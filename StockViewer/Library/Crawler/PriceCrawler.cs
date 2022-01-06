
namespace StockViewer.Library.Crawler;


public static class PriceCrawler
{
    [JsonIgnore]
    private static readonly HttpClient client = new HttpClient();

    public static void CrawlDate(DateTime target, Queue<DateTime> error)
    {
        try
        {
            Trace.WriteLine($"=========={target:yyyy/MM/dd}==========");
            string url = $"https://www.twse.com.tw/exchangeReport/MI_INDEX?response=json&date=" + $"{ target:yyyyMMdd}" + "&type=ALLBUT0999";
            Trace.WriteLine($"   - Send request: " + url);

            string? content = client.GetStringAsync(url).Result;

            if (content == null)
            {
                Trace.WriteLine($"   - Error!!!");
                error.Enqueue(target);
                error.SaveJson(FilePath.Path_Raw_Root, FilePath.Name_Error_Price);
                Trace.WriteLine($"   - Error Saved!");
                Trace.WriteLine($"   [Error] Can't catch data on {target:yyyy/MM/dd}");
            }
            else
            {
                content.SaveText(FilePath.Path_Raw_Price, $"{target:yyyyMMdd}");
                Trace.WriteLine($"   - Data Saved!");
            }
        }
        catch (Exception)
        {
            Trace.WriteLine($"   - Error!!!");
            error.Enqueue(target);
            error.SaveJson(FilePath.Path_Raw_Root, FilePath.Name_Error_Price);
            Trace.WriteLine($"   - Error Saved!");
            Trace.WriteLine($"   [Error] Can't catch data on {target:yyyy/MM/dd}");
        }
    }
    public static void Crawl(Queue<DateTime> error, DateTime? begin = null, DateTime? end = null)
    {
        DateTime target = begin ?? new(2004, 2, 13);
        DateTime now = end ?? DateTime.Now;

        while (target.AddHours(14) < now)
        {
            CrawlDate(target, error);
            target.SaveJson(FilePath.Path_Raw_Root, FilePath.Name_UpdateTime_Price);

            Thread.Sleep(2500);
            target = target.AddDays(1);
        }
        Trace.WriteLine($"========== Error ==========");
        Trace.WriteLine($"   - Error Count:{error.Count()}");
        error.SaveJson(FilePath.Path_Raw_Root, FilePath.Name_Error_Price);
        Trace.WriteLine($"   - Error Saved!");
    }
}
