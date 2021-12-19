using System.Net.Http;
using System.Net.Http.Json;

namespace mApp.MVVM.Model;

public class WebDatas
{
    private static readonly HttpClient client = new HttpClient();
    public List<List<string>>? data8 { get; set; }
    public List<List<string>>? data9 { get; set; }
    public List<string>? fields9 { get; set; }
    public static void Update(string dataPath)
    {
        DateTime limitTime = DateTime.Now;

        DateTime updateTime = UpdateTime.GetLocalLastUpdate(dataPath);
        DateTime targetTime = updateTime;
        StockDataGroup stockDataGroup = new($"{targetTime:yyyy}");

        while (targetTime.AddDays(1) < limitTime)
        {

            if (stockDataGroup.IsDifferentYear(targetTime.AddDays(1)))
            {
                Trace.WriteLine($"   Save stocksCollection in {stockDataGroup.NowUpdateYear:yyyy}");
                stockDataGroup.SaveToLocalDatas(dataPath);
                UpdateTime.SaveToLocalDatas(updateTime, dataPath);
                stockDataGroup = new($"{targetTime.AddDays(1):yyyy}");
            }

            targetTime = targetTime.AddDays(1);
            Trace.WriteLine($"============== {targetTime:yyyy/MM/dd} ==============");

        Reconnect:
            UpdateOption option = DownloadDatas(targetTime, dataPath, stockDataGroup);

            switch (option)
            {
                case UpdateOption.Data:
                    Trace.WriteLine($"   Download stocksCollection Done");
                    updateTime = targetTime;
                    Trace.WriteLine($"   Update LastDay");
                    break;
                case UpdateOption.Empty:
                    Trace.WriteLine($"   Stock Close Day");
                    updateTime = targetTime;
                    Trace.WriteLine($"   Update LastDay");
                    break;
                case UpdateOption.LostConnect:
                    Trace.WriteLine($"   [Error] Lost Connect... Waiting For Reconnecting...3");
                    System.Threading.Thread.Sleep(1350);
                    Trace.WriteLine($"   [Error] Lost Connect... Waiting For Reconnecting...2");
                    System.Threading.Thread.Sleep(1350);
                    Trace.WriteLine($"   [Error] Lost Connect... Waiting For Reconnecting...1");
                    System.Threading.Thread.Sleep(1350);
                    Trace.WriteLine($"   [Error] Reconnecting...");
                    goto Reconnect;
            }
            System.Threading.Thread.Sleep(4000);
        }

        stockDataGroup.SaveToLocalDatas(dataPath);
        UpdateTime.SaveToLocalDatas(updateTime, dataPath);

        static UpdateOption DownloadDatas(DateTime date, string dataPath, StockDataGroup stockDataGroup)
        {
            WebDatas? resDatas;
            try
            {
                Trace.WriteLine($"   - Load website {date:yyyy/MM/dd}");
                Trace.WriteLine($"   - Send request:{"https://www.twse.com.tw/exchangeReport/MI_INDEX?response=json&date=" + $"{ date:yyyyMMdd}" + "&type=ALLBUT0999"}");
                resDatas = HttpClientJsonExtensions.GetFromJsonAsync<WebDatas?>(client, "https://www.twse.com.tw/exchangeReport/MI_INDEX?response=json&date=" + $"{ date:yyyyMMdd}" + "&type=ALLBUT0999").Result;
            }
            catch (Exception)
            {
                Trace.WriteLine($"   [Error] Can't load website {date:yyyy/MM/dd}");
                return UpdateOption.LostConnect;
            }

            List<List<string>>? datalist = (UpdateTime.isBeforeSwitchDay(date)) ? resDatas?.data8 : resDatas?.data9;

            if (datalist != null)
            {
                Trace.WriteLine($"   - Get website data8/9");
                datalist.ForEach(data =>
                {
                    if (double.TryParse(data[5].Replace(",", ""), out _))
                    {
                        Stock stock = new(data[0], data[1]);

                        stock.TradingData.Add(date, new()
                        {
                            Volume = data[2],
                            VolumeMoney = data[4],
                            Start = data[5],
                            Max = data[6],
                            Min = data[7],
                            End = data[8],
                            Grade = data[9],
                            Spread = data[10],
                            Turnover = data[3]
                        });
                        stockDataGroup.AddGroup(stock);
                    }
                });
                return UpdateOption.Data;
            }
            else
            {
                Trace.WriteLine($"   - Empty data");
                return UpdateOption.Empty;
            }

        }
    }
}

internal enum UpdateOption
{
    Data,
    Empty,
    LostConnect
}

public class StockDataGroup
{
    public string NowUpdateYear { get; set; }
    public Dictionary<string, Stock> Group { get; set; } = new();
    public StockDataGroup(string year) => NowUpdateYear = year;
    public void AddGroup(Stock stock)
    {
        if (Group.ContainsKey(stock.Id))
            Group[stock.Id].AddTradingDatas(stock);
        else
            Group.Add(stock.Id, stock);
    }
    public bool IsDifferentYear(DateTime date) => NowUpdateYear != $"{date:yyyy}";
    public void SaveToLocalDatas(string dataPath)
    {
        foreach ((string id, Stock mStock) in Group)
        {
            Stock? stock = Json.LoadJsonData<Stock>(Path.Combine(dataPath, id), NowUpdateYear);

            if (stock == null)
                stock = new(mStock.Id, mStock.Name);

            stock.AddTradingDatas(mStock);
            Json.SaveDatasAsJson(stock, Path.Combine(dataPath, id), NowUpdateYear);
        }
    }
}

public static class UpdateTime
{
    public static readonly DateTime Beginning = new(2004, 2, 11, 14, 0, 0);
    // data8 before 2011/7/31, data9 since 2011/8/1
    public static readonly DateTime SwitchDay = new(2011, 7, 31, 14, 0, 0);
    public static bool isBeforeSwitchDay(DateTime date) => date < SwitchDay;
    public static void SaveToLocalDatas(DateTime date, string dataPath) => Json.SaveDatasAsJson(date, dataPath, "Update");
    public static DateTime GetLocalLastUpdate(string dataPath) {
        DateTime result = Json.LoadJsonData<DateTime>(dataPath, "Update");

        return result < Beginning ? Beginning : result;
    }
}