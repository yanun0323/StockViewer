
namespace StockViewer.MVVM.ViewModel;
public class MainViewModel : ObservableObject
{
    private static readonly double _ChartBarWidth_Max = 30;
    private static readonly double _ChartBarWidth_Min = 3;

    private readonly string _DefaultStockId = "2330";
    private string _searchWords = "";
    private Grid? MainChartGrid;
    private Point? MouseClickPosition;
    private ObservableCollection<PickStockBlockViewModel> _SmartPickVMCollection = new();
    private double _ChartBarWidth = 10;

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

        MainCrawler.Run();
        MainConverter.Run();

        _mStockModel = new();
        UpdateStock(_DefaultStockId);
        SmartPick();

        StockList = GenerateStockList();

        MainChartVM = new(mStockModel!);
        SubChartVM1 = new(mStockModel!, InstitutionOption.TrustSuper);
        SubChartVM2 = new(mStockModel!, InstitutionOption.ForeignSuper);

        MouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(e =>
        {
            double step = (_ChartBarWidth > 20) ? 3 : 1;
            double scale = ((e.Delta > 0) ? 1 : -1) * step;
            if (_ChartBarWidth + scale > _ChartBarWidth_Min && _ChartBarWidth + scale < _ChartBarWidth_Max)
            {
                _ChartBarWidth += scale;
                MainChartVM.SetBarWidth(_ChartBarWidth);
                SubChartVM1.SetBarWidth(_ChartBarWidth);
                SubChartVM2.SetBarWidth(_ChartBarWidth);

            }

            if (!MainChartVM.BarSizeChanged())
            {
                _ChartBarWidth -= scale;
                MainChartVM.SetBarWidth(_ChartBarWidth);
                SubChartVM1.SetBarWidth(_ChartBarWidth);
                SubChartVM2.SetBarWidth(_ChartBarWidth);
            }
            else
            {
                SubChartVM1.BarSizeChanged();
                SubChartVM2.BarSizeChanged();
            }
        });

        MouseMoveCommand = new RelayCommand<MouseEventArgs>(e =>
        {
            if (e.LeftButton == MouseButtonState.Pressed && MouseClickPosition != null)
            {
                Point pos = e.MouseDevice.GetPosition(MainChartGrid);
                MainChartVM.MouseDrag(pos);
                SubChartVM1.MouseDrag(pos);
                SubChartVM2.MouseDrag(pos);
            }
            else
            {
                MouseClickPosition = e.MouseDevice.GetPosition(MainChartGrid);
                MainChartVM.SetMouseClickPosition(MouseClickPosition!.Value);
                SubChartVM1.SetMouseClickPosition(MouseClickPosition!.Value);
                SubChartVM2.SetMouseClickPosition(MouseClickPosition!.Value);
            }
        });

        LoadedCommand = new RelayCommand<Grid>(obj =>
        {
            MainChartGrid = obj;
            Trace.WriteLine("LoadedCommand");
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
        MainChartVM?.UpdateChart(mStockModel!);
        SubChartVM1?.UpdateChart(mStockModel!);
        SubChartVM2?.UpdateChart(mStockModel!);
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

