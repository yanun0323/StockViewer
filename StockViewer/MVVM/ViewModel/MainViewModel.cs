
namespace StockViewer.MVVM.ViewModel;
public class MainViewModel : ObservableObject
{
    private readonly string _DefaultStockId = "2330";
    private string _searchWords = "";
    private Grid? MainChartGrid;
    private Point? MouseClickPosition;
    private ObservableCollection<PickStockBlockViewModel> _SmartPickVMCollection = new();
    private BarParameter _BarParam;

    private StockModel _mStockModel = new();
    public ICommand? MouseWheelCommand { get; set; }
    public ICommand? MouseMoveCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    
    public StockModel mStockModel
    {
        get { return _mStockModel; }
        set
        {
            _mStockModel = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<PickStockBlockViewModel> SmartPickVMCollection
    {
        get { return _SmartPickVMCollection; }
        set { _SmartPickVMCollection = value; OnPropertyChanged(); }
    }

    public Dictionary<char, HashSet<string>> StockList { get; set; } = new();
    public MainChartViewModel? MainChartVM { get; set; }
    public SubChartViewModel? SubChartVM1 { get; set; }
    public SubChartViewModel? SubChartVM2 { get; set; }
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
        _BarParam = new(60, 10, 10, 10);
        MainCrawler.Run();
        MainConverter.Run();
        Server.Connect();

        _mStockModel = new();
        UpdateStock(_DefaultStockId);
        SmartPick();

        StockList = GenerateStockList();

        MainChartVM = new(mStockModel!, new(() =>_BarParam));
        SubChartVM1 = new(mStockModel!, new(() => _BarParam));
        SubChartVM2 = new(mStockModel!, new(() => _BarParam));

        MouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(e =>
        {
            int step = (e.Delta < 0 ? 1 : -1) * (_BarParam.Count > 20 ? 5 : 1);
            _BarParam.Count += + step;
            RepaintChart();
        });

        MouseMoveCommand = new RelayCommand<MouseEventArgs>(e =>
        {
            if (e.LeftButton == MouseButtonState.Pressed && MouseClickPosition != null)
            {
                Point pos = e.MouseDevice.GetPosition(MainChartGrid);
                Trace.WriteLine($"pos {pos.X}");
                _BarParam.Start += (int)(pos.X / (_BarParam.Width + 2));
                RepaintChart();
            }
            else
            {
                MouseClickPosition = e.MouseDevice.GetPosition(MainChartGrid);
            }
        });

        LoadedCommand = new RelayCommand<Grid>(obj =>
        {
            MainChartGrid = obj;    
        });
    }

    private void SmartPick()
    {
        if (mStockModel!.LastDate == null)
            return;

        DateTime target = mStockModel!.LastDate!.Value;
        Dictionary<string, StockModel> stockModelCollection = new();
        PriceConverter.Catch(target, stockModelCollection);
        InstitutionConverter.Catch(target, stockModelCollection);
        HashSet<StockModel> list = stockModelCollection.Select(x => x.Value).Where(x => x.InstitutionData.Any()).ToHashSet();

        _SmartPickVMCollection = new();
        int count = 0;
        while (count++ < 100)
        {
            StockModel picked = list.MaxBy(x => x.InstitutionData[target].mTrustSuper)!;
            if (picked.InstitutionData[target].mTrustSuper <= 0)
                break;
            list.Remove(picked!);
            _SmartPickVMCollection!.Add(new(picked, new(() => this)));
        }
        SmartPickVMCollection = _SmartPickVMCollection;
    }

    public void UpdateStock(string stockId)
    {
        _mStockModel!.Refresh(stockId);
        mStockModel = _mStockModel;
        MainChartVM?.Update(mStockModel);
        SubChartVM1?.Update(mStockModel);
        SubChartVM2?.Update(mStockModel);
    }
    public void RepaintChart()
    {
        MainChartVM?.Update();
        SubChartVM1?.Update();
        SubChartVM2?.Update();
    }

    static Dictionary<char, HashSet<string>> GenerateStockList()
    {
        Dictionary<char, HashSet<string>> result = FileManagement.LoadJson<Dictionary<char, HashSet<string>>?>(FilePath.Path_StockList, FilePath.Name_StockList) ?? new();

        var folders = new DirectoryInfo(FilePath.Path_Stock).EnumerateDirectories("*");

        if (result.ContainsKey(' ') && result[' '].Count() == folders.Count())
            return result;

        result = new();
        foreach (DirectoryInfo folder in folders)
        {
            StockModel? stockModel = FileManagement.LoadJson<StockModel?>(Path.Combine(FilePath.Path_Stock, folder.Name), FirstFileName(folder));
            if (stockModel != null) 
            {
                string idName = stockModel.IdName;
                foreach (char cha in idName)
                {
                    if (!result.ContainsKey(cha))
                        result.Add(cha, new());

                    if (!result[cha].Contains(stockModel.IdName))
                        result[cha].Add(stockModel.IdName);
                }
            }
        }
        result.SaveJson(FilePath.Path_StockList, FilePath.Name_StockList);
        return result;

        static string FirstFileName(DirectoryInfo folder) => folder.GetFiles()[0].Name;
    }
    void Search() 
    {
        if(string.IsNullOrEmpty(SearchWords))
            return;
    }
}

