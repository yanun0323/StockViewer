
namespace StockViewer.MVVM.ViewModel;
public class MainViewModel : ObservableObject
{
    readonly string mDataPath = "";
    readonly string mDefaultStockId = "2330";
    private Stock? _displayStock = new();
    private TitleStock? _titleStock;
    private string _searchWords = "";

    public DateTime Update { get; }
    public HashSet<string> StockList { get; set; } = new();
    public MainChartViewModel? MainChartVM { get; set; }
    public Stock? DisplayStock 
    { 
        get => _displayStock; 
        set 
        { 
            _displayStock = value; 
            OnPropertyChanged(); 
        } 
    }
    public TitleStock? TitleStock 
    { 
        get => _titleStock; 
        set 
        { 
            _titleStock = value; 
            OnPropertyChanged(); 
        } 
    }
    public string SearchWords
    { 
        get => _searchWords; 
        set 
        { 
            _searchWords = value;
            //Search();
            OnPropertyChanged(); 
        } 
    }

    public MainViewModel()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        mDataPath = Path.Combine(path, Path.Combine("StockViewer", "Data"));

        WebDatas.Update(mDataPath);
        //WebDatas.UpdateParticularDate(mDataPath, 2018, 8, 22);

        Update = UpdateTime.GetLocalLastUpdate(mDataPath);
        Update_DisplayStock(mDefaultStockId);
        StockList = GenerateStockList(mDataPath);

        //WebDatas.CheckCorrect(mDataPath, _displayStock);

        MainChartVM = new(DisplayStock!);
        Trace.WriteLine("Datas Initialize");
    }
    public void Update_DisplayStock(string stockId)
    {
        Trace.WriteLine($"update DisplayStock {stockId}");
        DisplayStock = Stock.LoadLocalData(mDataPath, stockId);
        if (DisplayStock.Id == "")
            DisplayStock = Stock.LoadLocalData(mDataPath, mDefaultStockId);

        Trace.WriteLine($"DisplayStock.TradingData.Count() {DisplayStock!.TradingData.Count()}");
        Update_TitleStock(Update);
        MainChartVM?.UpdateChart(DisplayStock);
    }
    public void Update_TitleStock(DateTime updateTime)
    {
        Trace.WriteLine($"update TitleStock {_displayStock!.Id}");
        TitleStock = new(_displayStock!);
    }
    HashSet<string> GenerateStockList(string dataPath)
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
    void Search() 
    {
        if(string.IsNullOrEmpty(SearchWords))
            return;
    }
}

