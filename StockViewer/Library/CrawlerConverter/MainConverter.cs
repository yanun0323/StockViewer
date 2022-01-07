using System.Threading.Tasks;

namespace StockViewer.Library.CrawlerConverter;
public static class MainConverter
{
    public static void Run() 
    {
        //Dictionary<string, StockModel> stockModelCollection = new();
        //LoadStockModelCollection();
        DateTime? load = FileManagement.LoadJson<DateTime?>(FilePath.Path_Root, FilePath.Name_UpdateTime);
        DateTime start = (load != null) ? load.Value.AddDays(1) : PriceCrawler.Begin;
        DateTime now = DateTime.Now;
        List<Task> tasks = new();
        while (start < now)
        {
            var stockModelCollection = LoadStockModelCollection(start);
            PriceConverter.Run(start, stockModelCollection);
            InstitutionConverter.Run(start, stockModelCollection);
            Trace.WriteLine($"{start} {stockModelCollection.Count()}");

            if (stockModelCollection.ContainsKey("2330"))
            {
                Trace.WriteLine($" - PriceData: {stockModelCollection["2330"].PriceData.Count()}");
                Trace.WriteLine($" - InstitutionData: {stockModelCollection["2330"].InstitutionData.Count()}");
                if (stockModelCollection["2330"].PriceData.Count() == 0 ||
                    stockModelCollection["2330"].InstitutionData.Count() == 0)
                {
                    Trace.WriteLine($" -------------------");
                }
            }
            else
            {
                Trace.WriteLine($" -------------------");
            }
            if (stockModelCollection.Any())
            {
                DateTime last = (start.Year == now.Year) ? now : new DateTime(start.Year + 1, 1, 1).AddDays(-1);
                last.SaveJson(FilePath.Path_Root, FilePath.Name_UpdateTime);
                SaveStockModelCollection(start, stockModelCollection);
            }
            start = new(start.Year + 1, 1, 1);
        }
        //Task.WaitAll(tasks.ToArray());
        //SaveStockModelCollection();


    }
    private static void SaveStockModelCollection(DateTime date, Dictionary<string, StockModel> stockModelCollection) 
    {
        foreach ((string id, StockModel stockModel) in stockModelCollection)
        {
            string stockPath = Path.Combine(FilePath.Path_Stock, id);
            string fileName = $"{date:yyyy}";
            stockModel.SaveJson(stockPath, fileName);
        }

    }
    private static Dictionary<string, StockModel> LoadStockModelCollection(DateTime date)
    {
        Dictionary<string, StockModel> stockModelCollection = new();
        if (!Directory.Exists(FilePath.Path_Stock))
            _ = Directory.CreateDirectory(FilePath.Path_Stock);

        DirectoryInfo path = new(FilePath.Path_Stock);
        foreach (FileInfo stockFolder in path.EnumerateFiles("*"))
        {
            string stockPath = Path.Combine(FilePath.Path_Stock, stockFolder.Name);
            string fileName = $"{date:yyyy}";
            StockModel? stockModel = FileManagement.LoadJson<StockModel?>(stockPath, fileName);
            if (stockModel != null)
                stockModelCollection.Add(stockModel.Id, stockModel);

        }

        return stockModelCollection;
    }
}

