
namespace StockViewer.MVVM.ViewModel;
public class MainViewModel : ObservableObject
{
    private readonly string defaultStockId = "2330";
    private string searchWords = "";
    private bool isPressLeave = false;
    private Grid? mainChartGrid;
    private Point? mouseClickPosition;
    private ObservableCollection<PickStockBlockViewModel> smartPickVMCollection = new();
    private BarParameter barParam = new(50, 5, 10, 7); // Count , MinCount , Width , MinWidth
    private StockModel stockModel = new();



    public ICommand? MouseWheelCommand { get; set; }
    public ICommand? MouseMoveCommand { get; set; }
    public ICommand? MouseUpCommand { get; set; }
    public ICommand? MouseEnterCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    


    public StockModel mStockModel { get => stockModel; set { stockModel = value; OnPropertyChanged(); }}
    public ObservableCollection<PickStockBlockViewModel> SmartPickVMCollection { get => smartPickVMCollection; set { smartPickVMCollection = value; OnPropertyChanged(); }}
    public Dictionary<char, HashSet<string>> StockList { get; set; } = new();
    public MainChartViewModel? MainChartVM { get; set; }
    public SubChartViewModel? SubChartVM1 { get; set; }
    public SubChartViewModel? SubChartVM2 { get; set; }
    public string SearchWords { get => searchWords; set { searchWords = value; OnPropertyChanged(); }}



    public MainViewModel()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        MainCrawler.Run();
        MainConverter.Run();
        Server.Connect();

        stockModel = new();
        UpdateStock(defaultStockId);
        SmartPick();

        StockList = GenerateStockList();

        MainChartVM = new(mStockModel!, new(() =>barParam));
        SubChartVM1 = new(mStockModel!, new(() => barParam));
        SubChartVM2 = new(mStockModel!, new(() => barParam));

        MouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(e =>
        {
            int step = (e.Delta < 0 ? 1 : -1) * (barParam.Count / 7);
            barParam.Count += + step;
            RepaintChart();
        });

        MouseMoveCommand = new RelayCommand<MouseEventArgs>(e =>
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            if(mouseClickPosition == null)
                mouseClickPosition = e.MouseDevice.GetPosition(mainChartGrid);

            Point pos = e.MouseDevice.GetPosition(mainChartGrid);
            int step = (int)((pos.X - mouseClickPosition.Value.X) / (barParam.Width + 2)) * 3 / 5;
            barParam.Start = step + barParam.StartTemp;
            RepaintChart();
        }); 
        
        MouseUpCommand = new RelayCommand<MouseEventArgs>(e =>
        {
            mouseClickPosition = null;
            barParam.StartTemp =barParam.Start;
        });

        MouseEnterCommand = new RelayCommand<MouseEventArgs>(e =>
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                return;

            mouseClickPosition = null;
            barParam.StartTemp = barParam.Start;
        });

        LoadedCommand = new RelayCommand<Grid>(obj =>
        {
            mainChartGrid = obj;    
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

        smartPickVMCollection = new();
        int count = 0;
        while (count++ < 100)
        {
            StockModel picked = list.MaxBy(x => x.InstitutionData[target].mTrustSuper)!;
            if (picked.InstitutionData[target].mTrustSuper <= 0)
                break;
            list.Remove(picked!);
            smartPickVMCollection!.Add(new(picked, new(() => this)));
        }
        SmartPickVMCollection = smartPickVMCollection;
    }

    public void UpdateStock(string stockId)
    {
        stockModel!.Refresh(stockId);
        mStockModel = stockModel;
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

