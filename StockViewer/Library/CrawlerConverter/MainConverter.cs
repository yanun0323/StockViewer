using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockViewer.Library.CrawlerConverter;
public static class MainConverter
{
    public static void Run() 
    {
        Dictionary<string, StockModel> stockModelCollection = new();
        LoadStockModelCollection();

        PriceConverter.Run(stockModelCollection);
        InstitutionConverter.Run(stockModelCollection);

        Trace.WriteLine($"PriceData.Count(): {stockModelCollection["2330"].PriceData.Count()}");
        Trace.WriteLine($"InstitutionData.Count(): {stockModelCollection["2330"].InstitutionData.Count()}");

        SaveStockModelCollection();

        void LoadStockModelCollection()
        {
            if (!Directory.Exists(FilePath.Path_Stock))
                _ = Directory.CreateDirectory(FilePath.Path_Stock);

            DirectoryInfo path = new(FilePath.Path_Stock);
            foreach (FileInfo file in path.EnumerateFiles("*"))
            {
                StockModel? stockModel = FileManagement.LoadJson<StockModel?>(FilePath.Path_Stock, file.Name);
                if (stockModel != null)
                    stockModelCollection.Add(stockModel.Id, stockModel);
            }
        }
        void SaveStockModelCollection() 
        {
            foreach (var pair in stockModelCollection)
            {
                StockModel stockModel = stockModelCollection[pair.Key];
                stockModel.SaveJson(FilePath.Path_Stock, stockModel.Id);
            }
        }
    }


    
}
