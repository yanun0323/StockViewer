
namespace StockViewer.MVVM.ViewModel;
public class MainViewModel : ObservableObject
{
    readonly string _DefaultStockId = "2330";

    private string mDataPath = "";
    private StockModel? _mStockModel;

    public StockModel? mStockModel
    {
        get { return _mStockModel; }
        set
        {
            _mStockModel = value;
            OnPropertyChanged();
        }
    }

    private Stock? _displayStock = new();
    private string _searchWords = "";

    private ObservableCollection<PickStockBlockViewModel> _PickStockVMCollection;

    public ObservableCollection<PickStockBlockViewModel> PickStockVMCollection
    {
        get { return _PickStockVMCollection; }
        set { _PickStockVMCollection = value; OnPropertyChanged(); }
    }

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
        mDataPath = Path.Combine(path, Path.Combine("StockViewer\\Data", "Price"));

        MainCrawler.Run();
        MainConverter.Run();

        mStockModel = new(id:_DefaultStockId);
        mStockModel.Refresh();

        WebDatas.Update(mDataPath);

        Update = Model.Update.GetLocalLastUpdate(mDataPath);
        Update_DisplayStock(_DefaultStockId);
        StockList = GenerateStockList(mDataPath);

        //WebDatas.CheckCorrect(mDataPath, _displayStock);

        MainChartVM = new(DisplayStock!);
        Trace.WriteLine("Datas Initialize");

        _PickStockVMCollection = new();
        int count = 0;
        while (count++ < 20) {
            _PickStockVMCollection!.Add(new(mStockModel));
        }
        PickStockVMCollection = _PickStockVMCollection;
    }
    public void Update_DisplayStock(string stockId)
    {
        DisplayStock = Stock.LoadLocalData(mDataPath, stockId);
        if (DisplayStock.Id == "")
            DisplayStock = Stock.LoadLocalData(mDataPath, _DefaultStockId);

        MainChartVM?.UpdateChart(DisplayStock);
    }
    HashSet<string> GenerateStockList(string dataPath)
    {
        HashSet<string> result = new();
        DirectoryInfo path = new(dataPath);
        foreach (DirectoryInfo folder in path.EnumerateDirectories("*"))
        {
                Stock? stock = FileManagement.LoadJson<Stock>(Path.Combine(dataPath, folder.Name), folder.GetFiles()[0].Name);
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

