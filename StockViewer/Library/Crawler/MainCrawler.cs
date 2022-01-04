
namespace StockViewer.Library.Crawler;
public class MainCrawler
{
    public static void Run()
    {
        CrawlPrice();
        CrawlInstitution();

        ReCrawlPrice();
        ReCrawlInstitution();


        static void CrawlPrice()
        {
            Console.WriteLine($"========== CrawlPrice ==========");
            Queue<DateTime> error = Extention.LoadJson<Queue<DateTime>>(FilePath.PathRoot, FilePath.NamePriceError) ?? new();

            DateTime? beginPrice = Extention.LoadJson<DateTime?>(FilePath.PathRoot, FilePath.NamePriceUpdateTime);
            if (beginPrice != null) beginPrice = beginPrice.Value.AddDays(1);
            PriceCrawler.Crawl(error, begin: beginPrice);
        }
        static void CrawlInstitution()
        {
            Console.WriteLine($"========== CrawlInstitution ==========");
            Queue<DateTime> error = Extention.LoadJson<Queue<DateTime>>(FilePath.PathRoot, FilePath.NameInstitutionError) ?? new();

            DateTime? beginInstitution = Extention.LoadJson<DateTime?>(FilePath.PathRoot, FilePath.NameInstitutionUpdateTime);
            if (beginInstitution != null) beginInstitution = beginInstitution.Value.AddDays(1);

            InstitutionCrawler.Crawl(error, begin: beginInstitution);
        }
        static void ReCrawlPrice()
        {
            Console.WriteLine($"========== ReCrawlPrice ==========");
            Queue<DateTime> error = new();
            Queue<DateTime> errorPrice = Extention.LoadJson<Queue<DateTime>>(FilePath.PathRoot, FilePath.NamePriceError) ?? new();
            while (errorPrice.Any())
            {
                PriceCrawler.CrawlDate(errorPrice.Dequeue(), error);
                Thread.Sleep(2500);
                errorPrice.SaveJson(FilePath.PathRoot, FilePath.NamePriceError);
            }
            Console.WriteLine($"========== Error ==========");
            Console.WriteLine($"   - Error Count:{error.Count()}");
            error.SaveJson(FilePath.PathRoot, FilePath.NamePriceError);
            Console.WriteLine($"   - Error Saved!");
        }
        static void ReCrawlInstitution()
        {
            Console.WriteLine($"========== ReCrawlInstitution ==========");
            Queue<DateTime> error = new();
            Queue<DateTime> errorInstitutionError = Extention.LoadJson<Queue<DateTime>>(FilePath.PathRoot, FilePath.NameInstitutionError) ?? new();
            while (errorInstitutionError.Any())
            {
                PriceCrawler.CrawlDate(errorInstitutionError.Dequeue(), error);
                Thread.Sleep(2500);
                errorInstitutionError.SaveJson(FilePath.PathRoot, FilePath.NameInstitutionError);
            }
            Console.WriteLine($"========== Error ==========");
            Console.WriteLine($"   - Error Count:{error.Count()}");
            error.SaveJson(FilePath.PathRoot, FilePath.NameInstitutionError);
            Console.WriteLine($"   - Error Saved!");
        }
    }
}
