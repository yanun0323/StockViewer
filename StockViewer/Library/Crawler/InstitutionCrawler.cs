namespace StockViewer.Library.Crawler;


public static class InstitutionCrawler
{
    private static readonly HttpClient client = new();
    public static DateTime Begin = new(2012, 5, 2);

    public static void CrawlDate(DateTime target, Queue<DateTime> error)
    {
        try
        {
            Trace.WriteLine($"=========={target:yyyy/MM/dd}==========");
            string url = $"https://www.twse.com.tw/fund/T86?response=json&date=" + $"{target:yyyyMMdd}" + "&selectType=ALL";
            Trace.WriteLine($"   - Send request: " + url);

            string? content = client.GetStringAsync(url).Result;

            if (content == null)
            {
                RefreshError(target, error);
                SaveError(error);
            }
            else
            {
                content.SaveText(FilePath.Path_Raw_Institution, $"{target:yyyyMMdd}");
                Trace.WriteLine($"   - Data Saved!");
            }
        }
        catch (Exception)
        {
            RefreshError(target, error);
            SaveError(error);
        }

        static void RefreshError(DateTime target, Queue<DateTime> error)
        {
            Trace.WriteLine($"   - Error!!!");
            error.Enqueue(target);
        }
    }
    public static void Crawl(Queue<DateTime> error, DateTime? begin = null, DateTime? end = null)
    {
        DateTime target = begin ?? Begin;
        DateTime now = end ?? DateTime.Now;

        while (target.AddHours(17) < now)
        {
            CrawlDate(target, error);
            target.SaveJson(FilePath.Path_Raw_Root, FilePath.Name_UpdateTimeName_Institution);

            Thread.Sleep(2500);
            target = target.AddDays(1);
        }
        Trace.WriteLine($"========== Error ==========");
        Trace.WriteLine($"   - Error Count:{error.Count()}");

        SaveError(error);
    }

    private static void SaveError(Queue<DateTime> error)
    {
        error.SaveJson(FilePath.Path_Raw_Root, FilePath.Name_Error_Institution);
        Trace.WriteLine($"   - Error Saved!");
    }


}


