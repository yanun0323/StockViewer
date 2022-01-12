using System.Threading.Tasks;

namespace StockViewer.Library.CrawlerConverter;
public static class MainConverter
{
    public static void Run() 
    {
        DateTime? load = FileManagement.LoadJson<DateTime?>(FilePath.Path_Data, FilePath.Name_UpdateTime);
        DateTime start = (load != null) ? load.Value.AddDays(1) : PriceCrawler.Begin;
        DateTime now = DateTime.Now;
        List<Task> tasks = new();
        while (start < now)
        {
            DateTime copy = start.AddDays(0);
            Task task = new(()=>
            {
                var stockModelCollection = LoadStockModelCollection(copy);
                PriceConverter.Run(copy, stockModelCollection);
                InstitutionConverter.Run(copy, stockModelCollection);
                if (stockModelCollection.Any())
                    SaveStockModelCollection(copy, stockModelCollection);
            }
            );
            task.Start();
            tasks.Add(task);
            start = new(start.Year + 1, 1, 1);
        }
        Task.WhenAll(tasks).Wait();
        SaveUpdateTime(start, now);
    }
    private static void SaveUpdateTime(DateTime start, DateTime now)
    {
        DateTime last = (start.Year == now.Year) ? now : new DateTime(start.Year + 1, 1, 1).AddDays(-1);
        last.SaveJson(FilePath.Path_Data, FilePath.Name_UpdateTime);
    }
    private static void SaveStockModelCollection(DateTime date, Dictionary<string, StockModel> stockModelCollection)
    {
        string fileName = $"{date:yyyy}";
        Trace.WriteLine($"Saving... {fileName}");
        foreach ((string id, StockModel stockModel) in stockModelCollection)
        {
            string stockPath = Path.Combine(FilePath.Path_Stock, id);
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

