namespace mAPP.ViewModel;
public class MainViewModel : INotifyPropertyChanged
{
    readonly string mDataPath = @"D:\YStock\Data";
    readonly string mDefaultStockId = "2330";

    public HashSet<string> StockList { get; set; } = new();
    public Stock? DisplayStock { get => _displayStock; set { _displayStock = DisplayStock; Update_TitleStock(Update); OnPropertyChanged(nameof(DisplayStock)); } }
    private Stock? _displayStock = new();
    public TitleStock? TitleStock { get => _titleStock; set { _titleStock = TitleStock; OnPropertyChanged(nameof(TitleStock)); } }
    private TitleStock? _titleStock;
    public DateTime Update  { get; }

    public MainViewModel()
    {
        WebDatas.Update(mDataPath);
        Update = UpdateTime.GetLocalLastUpdate(mDataPath);
        Update_DisplayStock(mDefaultStockId);
        StockList = GenerateStockList(mDataPath);
        Trace.WriteLine("Datas Initialize");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public void Update_DisplayStock(string stockId)
    {
        Trace.WriteLine($"update DisplayStock {stockId}");
            _displayStock = Stock.LoadLocalData(mDataPath, stockId);
        if (_displayStock.Id == "")
            _displayStock = Stock.LoadLocalData(mDataPath, mDefaultStockId);

        DisplayStock = _displayStock;
    }
    public void Update_TitleStock(DateTime updateTime)
    {
        Trace.WriteLine($"update TitleStock {_displayStock!.Id}");
        TradingData newestData;
        if(_displayStock!.TradingData.TryGetValue(updateTime, out newestData))
            _titleStock = new(newestData);
        else
            _titleStock = new(new()
            {
                Volume = "0",
                VolumeMoney = "0",
                Start = "0",
                Max = "0",
                Min = "0",
                End = "0",
                Grade = "0",
                Spread = "0",
                Turnover = "0"
            });



        TitleStock = _titleStock;
    }

    static HashSet<string> GenerateStockList(string dataPath)
    {
        HashSet<string> result = new();
        DirectoryInfo path = new(dataPath);
        foreach (DirectoryInfo folder in path.EnumerateDirectories("*"))
        {
                Stock? stock = Json.LoadJsonData<Stock>(Path.Combine(dataPath, folder.Name), folder.GetFiles()[0].Name);
                if (stock != null)
                result.Add(string.Join(" ", stock.Id , stock.Name));
        }
        return result;
    }
}

