namespace StockViewer.MVVM.ViewModel;
public class SubChartViewModel : ObservableObject
{
    private Size _ChartSize = new(854, 77.5);
    private double _BarWidth = 10;
    private int ChartLineQuantityRatio = MainChartViewModel.ChartLineQuantityRatio;

    private Grid? _SubChartGrid;
    private Point? _MouseClickPosition;
    private StockModel? _mStockModel;
    private ChartGridViewModel? _BarGridVM;
    private double _HighestPrice;
    private double _LowestPrice;

    public ICommand? LoadedCommand { get; set; }
    public ICommand? SizeChangedCommand { get; set; }
    public double mGridWidth { get => MainChartViewModel.GridWidth; }
    public ChartStructure<BarViewModel> BarVMStruct { get; set; }
        = new(new((BarViewModel b) => (b.Insti.mTrustSuper, b.Insti.mTrustSuper, b.mDate)));
    public ChartGridViewModel? BarGridVM { get => _BarGridVM; set { _BarGridVM = value; OnPropertyChanged(); } }
    
    public StockModel mStockModel
    {
        get => _mStockModel ?? new();
        set
        {
            _mStockModel = value;
            Update();
            OnPropertyChanged();
        }
    }

    public SubChartViewModel(StockModel stockModel, InstitutionOption option)
    {
        mStockModel = stockModel; 
        
        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(args =>
        {
            if (_ChartSize.Width == args.NewSize.Width)
            {
            _ChartSize = args.NewSize;
                ResizeBar();
            }
            else
            {
                _ChartSize = args.NewSize;
                BarSizeChanged();
            }
        });

        LoadedCommand = new RelayCommand<Grid>(obj =>
        {
            _SubChartGrid = obj;
        });
    }

    public void UpdateChart(StockModel stockModel) => mStockModel = stockModel;
    public void SetBarWidth(double width) => _BarWidth = width;
    public void SetMouseClickPosition(Point pos) => _MouseClickPosition = pos; 
    public bool BarSizeChanged()
    {
        var newCandleCount = GetNewBarCount();
        var candleCount = BarVMStruct!.Count();
        if (newCandleCount > candleCount)
        {
            var addCount = newCandleCount - candleCount;
            BarVMStruct.ZoomOut(addCount);
        }
        else if (newCandleCount < candleCount)
        {
            var reduceCount = candleCount - newCandleCount;
            BarVMStruct.ZoomIn(reduceCount);
        }
        ResizeBar();
        _BarGridVM!.Resize(_ChartSize);
        BarGridVM = _BarGridVM;
        return true;
    }
    public void MouseDrag(Point pos)
    {
        if (BarVMStruct.AllShow)
            return;

        var count = GetCountFromCount(pos);
        if (count > 0)
            BarVMStruct.PanRight(count);
        else if (count < 0)
            BarVMStruct.PanLeft(-count);

        ResizeBar();

        if (count != 0)
            _MouseClickPosition = pos;

        int GetCountFromCount(Point _pos) => (int)((_MouseClickPosition!.Value.X - _pos.X) / _BarWidth);
        
    }

    private void Update()
    {
        Update_BarVMCollection_Stack();
        Queue<BarViewModel> show = new();
        var count = GetNewBarCount();
        while (count > 0)
        {
            BarVMStruct.Generate(count);
            count--;
        }
        ResizeBar();
    }
    private void Update_BarVMCollection_Stack()
    {
        foreach ((DateTime date, Price price) in mStockModel.PriceData)
        {
            if(mStockModel.InstitutionData.ContainsKey(date))
                BarVMStruct.Push(CreateBarVM(date, mStockModel.InstitutionData[date]));
            else
                BarVMStruct.Push(CreateBarVM(date));
        }
    }
    private int GetNewBarCount() => (int)(_ChartSize.Width / _BarWidth) + 1;
    private BarViewModel CreateBarVM(DateTime date, Institution? institution = null)
    {
        Institution insti = institution ?? Institution.Deafult();
        ChartParameter cp = new()
        {
            Width = _BarWidth,
            Height = _ChartSize.Height,
            Highest = 0,
            Lowest = 0,
        };
        return new BarViewModel(date, insti, cp, InstitutionOption.TrustSuper);
    }
    private void ResizeBar()
    {
        ResizeChartGrid(_ChartSize.Height);

        List<Task> tasks = new();
        foreach (BarViewModel barVm in BarVMStruct.Middle!)
        {
            var copy = barVm;
            Task task = new(() =>
            {
                copy.Resize(_ChartSize.Height, _BarWidth, _HighestPrice, _LowestPrice);
            });
            task.Start();
            tasks.Add(task);
        }
        Task.WhenAll(tasks).Wait();
        BarVMStruct.Refresh();
    }
    private void ResizeChartGrid(double chartHeight)
    {
        _HighestPrice = BarVMStruct.Max;
        _LowestPrice = BarVMStruct.Min;
        if (Math.Abs(_HighestPrice) > Math.Abs(_LowestPrice))
            _LowestPrice = -_HighestPrice;
        else
            _HighestPrice = -_LowestPrice;


        if (_HighestPrice == 0 && _LowestPrice == 0)
        {
            _BarGridVM = new ChartGridViewModel(new ChartParameter()
            {
                Highest = 1,
                Lowest = -1,
                Width = _ChartSize.Width,
                Height = _ChartSize.Height,
            }, true, 1, 0, -1);
        }
        else
        {
            _BarGridVM = new ChartGridViewModel(new ChartParameter()
            {
                Highest = _HighestPrice,
                Lowest = _LowestPrice,
                Width = _ChartSize.Width,
                Height = _ChartSize.Height,
            }, true, _HighestPrice, _HighestPrice / 2, 0, _LowestPrice / 2, _LowestPrice);
        }
        BarGridVM = _BarGridVM;

    }


}
