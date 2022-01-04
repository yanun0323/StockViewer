using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading;

namespace StockViewer.Library.Crawler;
public class PriceCrawler
{
    [JsonIgnore]
    private static readonly HttpClient client = new HttpClient();
    public string date { get; set; } = "";
    public List<List<string>> data8 { get; set; } = new();
    public List<List<string>> data9 { get; set; } = new();
    public List<string> fields9 { get; set; } = new();

    public static void CrawlDate(DateTime target, Queue<DateTime> error)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        PriceCrawler? WebsiteData;
        try
        {
            Console.WriteLine($"=========={target:yyyy/MM/dd}==========");
            string url = $"https://www.twse.com.tw/exchangeReport/MI_INDEX?response=json&date=" + $"{ target:yyyyMMdd}" + "&type=ALLBUT0999";
            Console.WriteLine($"   - Send request: " + url);
            string json = client.GetStringAsync(url).Result;
            WebsiteData = JsonSerializer.Deserialize<PriceCrawler?>(json);

            if (WebsiteData == null || WebsiteData.date == "" || (!WebsiteData.data8.Any() && !WebsiteData.data9.Any()))
            {
                Console.WriteLine($"   - No Data");
                Console.WriteLine($"   - 沒有資料: {target:yyyy/MM/dd}");
            }
            else
            {
                string pathToSave = Path.Combine(FilePath.PathPrice, WebsiteData.date.Remove(4));
                WebsiteData.SaveJson(pathToSave, WebsiteData.date);
                Console.WriteLine($"   - Data Saved!");
            }
        }
        catch (Exception)
        {
            Console.WriteLine($"   - Error!!!");
            error.Enqueue(target);
            error.SaveJson(FilePath.PathRoot, FilePath.NamePriceError);
            Console.WriteLine($"   - Error Saved!");
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
            target.SaveJson(FilePath.PathRoot, FilePath.NamePriceUpdateTime);

            Thread.Sleep(2500);
            target = target.AddDays(1);
        }
        Console.WriteLine($"========== Error ==========");
        Console.WriteLine($"   - Error Count:{error.Count()}");
        error.SaveJson(FilePath.PathRoot, FilePath.NamePriceError);
        Console.WriteLine($"   - Error Saved!");
    }
}
