
namespace StockViewer.Library.Crawler;
public class MainCrawler
{
    public static void Run()
    {
        CrawlPrice();
        CrawlInstitution();

        ReCrawlPrice();
        ReCrawlInstitution();
    }
    private static void CrawlPrice()
    {
        Trace.WriteLine($"========== CrawlPrice ==========");
        Queue<DateTime> error = FileManagement.LoadJson<Queue<DateTime>>(FilePath.Path_Raw_Root, FilePath.Name_Error_Price) ?? new();

        DateTime? beginPrice = FileManagement.LoadJson<DateTime?>(FilePath.Path_Raw_Root, FilePath.Name_UpdateTime_Price);
        if (beginPrice != null) beginPrice = beginPrice.Value.AddDays(1);
        PriceCrawler.Crawl(error, begin: beginPrice);
    }
    private static void CrawlInstitution()
    {
        Trace.WriteLine($"========== CrawlInstitution ==========");
        Queue<DateTime> error = FileManagement.LoadJson<Queue<DateTime>>(FilePath.Path_Raw_Root, FilePath.Name_Error_Institution) ?? new();

        DateTime? beginInstitution = FileManagement.LoadJson<DateTime?>(FilePath.Path_Raw_Root, FilePath.Name_UpdateTimeName_Institution);
        if (beginInstitution != null) beginInstitution = beginInstitution.Value.AddDays(1);

        InstitutionCrawler.Crawl(error, begin: beginInstitution);
    }
    private static void ReCrawlPrice()
    {
        Trace.WriteLine($"========== ReCrawlPrice ==========");
        Queue<DateTime> error = new();
        Queue<DateTime> errorPrice = FileManagement.LoadJson<Queue<DateTime>>(FilePath.Path_Raw_Root, FilePath.Name_Error_Price) ?? new();
        while (errorPrice.Any())
        {
            PriceCrawler.CrawlDate(errorPrice.Dequeue(), error);
            Thread.Sleep(2500);
            errorPrice.SaveJson(FilePath.Path_Raw_Root, FilePath.Name_Error_Price);
        }
        Trace.WriteLine($"========== Error ==========");
        Console.WriteLine($"   - Error Count:{error.Count()}");
        error.SaveJson(FilePath.Path_Raw_Root, FilePath.Name_Error_Price);
        Console.WriteLine($"   - Error Saved!");
    }
    private static void ReCrawlInstitution()
    {
        Trace.WriteLine($"========== ReCrawlInstitution ==========");
        Queue<DateTime> error = new();
        Queue<DateTime> errorInstitutionError = FileManagement.LoadJson<Queue<DateTime>>(FilePath.Path_Raw_Root, FilePath.Name_Error_Institution) ?? new();
        while (errorInstitutionError.Any())
        {
            PriceCrawler.CrawlDate(errorInstitutionError.Dequeue(), error);
            Thread.Sleep(2500);
            errorInstitutionError.SaveJson(FilePath.Path_Raw_Root, FilePath.Name_Error_Institution);
        }
        Trace.WriteLine($"========== Error ==========");
        Console.WriteLine($"   - Error Count:{error.Count()}");
        error.SaveJson(FilePath.Path_Raw_Root, FilePath.Name_Error_Institution);
        Console.WriteLine($"   - Error Saved!");
    }
}

