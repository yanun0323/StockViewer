namespace StockViewer.MVVM.ViewModel;
public class SubChartViewModel : ObservableObject
{
    private Size _ChartSize = new(854, 77.5);
    private double _BarWidth = 10;
    private int ChartLineQuantityRatio = MainChartViewModel.ChartLineQuantityRatio;

    private Grid? _SubChartGrid;
    private Point? _MouseClickPosition;
    private StockModel? _mStockModel;
    private ObservableCollection<BarViewModel> _BarVMCollection = new();
    private Stack<BarViewModel> _BarVMCollection_Left = new();
    private Stack<BarViewModel> _BarVMCollection_Right = new();
    private ChartGridViewModel? _BarGridVM;
    private double _HighestPrice;
    private double _LowestPrice;

    public ICommand? LoadedCommand { get; set; }
    public ICommand? SizeChangedCommand { get; set; }
    public double mGridWidth { get => MainChartViewModel.GridWidth; }
    public ObservableCollection<BarViewModel> BarVMCollection { get => _BarVMCollection; set { _BarVMCollection = value; OnPropertyChanged(); } }
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
        var candleCount = _BarVMCollection!.Count();
        if (newCandleCount > candleCount)
        {
            var addCount = newCandleCount - candleCount;
            if (AddCandleLeft(addCount))
                ResizeBar();
            else
            {
                _BarGridVM!.Resize(_ChartSize);
                return false;
            }
        }
        else if (newCandleCount < candleCount)
        {
            var reduceCount = candleCount - newCandleCount;
            if (ReduceCandleLeft(reduceCount))
                ResizeBar();
            else
            {
                _BarGridVM!.Resize(_ChartSize);
                return false;
            }
        }
        _BarGridVM!.Resize(_ChartSize);
        BarGridVM = _BarGridVM;
        return true;

        bool ReduceCandleLeft(int reduceCount)
        {
            if (!_BarVMCollection!.Any() || _BarVMCollection!.Count() < reduceCount)
                return false;
            while (reduceCount > 0)
            {
                var candleVM = _BarVMCollection!.Last();
                _BarVMCollection_Left!.Push(candleVM);
                _BarVMCollection!.RemoveAt(_BarVMCollection!.Count() - 1);
                reduceCount--;
            }
            return true;
        }
        bool AddCandleLeft(int addCount)
        {
            if (!_BarVMCollection_Left!.Any() || _BarVMCollection_Left!.Count() < addCount)
                return false;
            while (addCount > 0)
            {
                var candleVM = _BarVMCollection_Left!.Pop();

                _BarVMCollection!.Add(candleVM);
                addCount--;
            }
            return true;
        }
    }
    public void MouseDrag(Point pos)
    {
        if (!_BarVMCollection_Left!.Any() && !_BarVMCollection_Right!.Any())
            return;

        var count = GetCountFromCount(pos);
        if (count > 0)
            ChartMoveRight(count);
        else if (count < 0)
            ChartMoveLeft(-count);

        if (count != 0)
            _MouseClickPosition = pos;

        int GetCountFromCount(Point _pos) => (int)((_MouseClickPosition!.Value.X - _pos.X) / _BarWidth);
        void ChartMoveLeft(int count)
        {
            while (count > 0 && _BarVMCollection_Left!.Any())
            {
                var candleVM = _BarVMCollection_Left!.Pop();
                _BarVMCollection!.Add(candleVM);

                candleVM = _BarVMCollection!.First();
                _BarVMCollection!.RemoveAt(0);
                _BarVMCollection_Right!.Push(candleVM);
                count--;
                ResizeBar();
            }
        }
        void ChartMoveRight(int count)
        {
            while (count > 0 && _BarVMCollection_Right!.Any())
            {
                var candleVM = _BarVMCollection_Right!.Pop();
                _BarVMCollection!.Insert(0, candleVM);

                candleVM = _BarVMCollection!.Last();
                _BarVMCollection!.RemoveAt(_BarVMCollection!.Count() - 1);
                _BarVMCollection_Left!.Push(candleVM);
                count--;
            }
            ResizeBar();
        }
    }

    private void Update()
    {
        Update_BarVMCollection_Stack();
        Queue<BarViewModel> show = new();
        var count = GetNewBarCount();
        while (count > 0 && _BarVMCollection_Left!.Any())
        {
            show.Enqueue(_BarVMCollection_Left!.Pop());
            count--;
        }
        _BarVMCollection = new();
        while (show.Any())
        {
            _BarVMCollection.Add(show.Dequeue());
        }
        ResizeBar();
    }
    private void Update_BarVMCollection_Stack()
    {
        _BarVMCollection_Right = new();
        _BarVMCollection_Left = new();
        foreach ((DateTime date, Price price) in mStockModel.PriceData)
        {
            if(mStockModel.InstitutionData.ContainsKey(date))
                _BarVMCollection_Left!.Push(CreateBarVM(date, mStockModel.InstitutionData[date]));
            else
                _BarVMCollection_Left!.Push(CreateBarVM(date));
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

        foreach (BarViewModel barVm in _BarVMCollection!)
        {
            barVm.Resize(_ChartSize.Height, _BarWidth, _HighestPrice, _LowestPrice);
        }
        BarVMCollection = _BarVMCollection;
    }
    private void ResizeChartGrid(double chartHeight)
    {
        _HighestPrice = _BarVMCollection!.Max(x => x.Insti.mTrustSuper);
        _LowestPrice = _BarVMCollection!.Min(x => x.Insti.mTrustSuper);
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
