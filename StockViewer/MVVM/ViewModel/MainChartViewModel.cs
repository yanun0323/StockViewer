
using System.Collections.ObjectModel;
using System.Windows.Shapes;

namespace StockViewer.MVVM.ViewModel;
public class MainChartViewModel:ObservableObject
{
    public ICommand? MouseUpCommand { get; set; }
    public ICommand? MouseMoveCommand { get; set; }
    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    public ICommand? CoordMouseMoveCommand { get; set; }


    public static readonly Thickness CandleMargin = new(1, 0, 1, 0);
    public static readonly int ChartLineQuantityRatio = 30;

    private Grid? _MainChartGrid;
    private Point? _MouseClickPosition;

    private double _HighestPrice;
    private double _LowestPrice;
    private StockModel? _mStockModel;

    private ObservableCollection<CandleViewModel>? _CandleVMCollection;
    private Stack<CandleViewModel>? _CandleVMCollection_Right;
    private Stack<CandleViewModel>? _CandleVMCollection_Left;

    private Size _ChartSize = new(854, 361);
    private double _CandleWidth = 10;
    private DateTime _TempLabelDate;
    private int _HighestVolume;
    private Visibility _InfoPopShow = Visibility.Hidden;
    private CandleViewModel? _InfoVM;
    private string _InfoDate = "";
    private ChartGridViewModel? _ChartGridVM;

    public static readonly double GridWidth = 50;
    public Visibility InfoPopShow { get => _InfoPopShow; set { _InfoPopShow = value; OnPropertyChanged(); } }
    public double CandleHeight { get => _ChartSize.Height; }
    public double CandleWidth { get => _CandleWidth; }
    public double _CandleOutlineWidth { get => _CandleWidth + CandleMargin.Left + CandleMargin.Right; }
    public ObservableCollection<CandleViewModel>? CandleVMCollection { get => _CandleVMCollection; set { _CandleVMCollection = value; OnPropertyChanged(); } }
    public CandleViewModel? InfoVM { get => _InfoVM; set { _InfoVM = value; OnPropertyChanged(); } }
    public ChartGridViewModel? ChartGridVM { get => _ChartGridVM; set { _ChartGridVM = value; OnPropertyChanged(); } }
    public double mGridWidth { get => GridWidth; }
    public string InfoDate { get => _InfoDate; set { _InfoDate = value; OnPropertyChanged(); } }
    public StockModel mStockModel
    {
        get => _mStockModel ?? new();
        set 
        { 
            _mStockModel = value;
            StockUpdate();
            OnPropertyChanged();
        }
    }

    public MainChartViewModel(StockModel stockModel)
    {
        mStockModel = stockModel;

        MouseUpCommand = new RelayCommand<MouseButtonEventArgs>(e => 
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                if (InfoPopShow != Visibility.Visible)
                {
                    Point pos = e.MouseDevice.GetPosition(_MainChartGrid);
                    double distanceFromRight = _ChartSize.Width - pos.X;
                    if (distanceFromRight > 0)
                    {
                        int index = (int)(distanceFromRight / _CandleOutlineWidth);
                        InfoVM = _CandleVMCollection!.ElementAt(index);
                        InfoDate = $"{InfoVM!.Date:yyyy/MM/dd}";
                    }
                    else
                    {
                        InfoVM = _CandleVMCollection!.ElementAt(0);
                        InfoDate = $"{InfoVM!.Date:yyyy/MM/dd}";
                    }
                }
                InfoPopShow = (InfoPopShow == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
            }
        });

        MouseMoveCommand = new RelayCommand<MouseEventArgs>(e =>
        {
            if (InfoPopShow == Visibility.Visible)
            {
                Point pos = e.MouseDevice.GetPosition(_MainChartGrid);
                double distanceFromRight = _ChartSize.Width - pos.X;
                if (distanceFromRight < 0)
                    return;

                int index = (int)(distanceFromRight / _CandleOutlineWidth);
                if (index >= _CandleVMCollection!.Count())
                    return;

                InfoVM = _CandleVMCollection!.ElementAt(index);
                InfoDate = $"{InfoVM!.Date:yyyy/MM/dd}";
            }
        });

        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(args =>
        {
            if (_ChartSize.Width == args.NewSize.Width)
            {
                _ChartSize = args.NewSize;
                ResizeCandle();
            }
            else
            {
                _ChartSize = args.NewSize;
                CandleSizeChanged();
            }
        });

        LoadedCommand = new RelayCommand<Grid>(obj =>
        {
            _MainChartGrid = obj;
        });
    }

    public void UpdateChart(StockModel stockModel) => mStockModel = stockModel;
    public void SetCandleWidth(double width) => _CandleWidth = width;
    public void SetMouseClickPosition(Point pos) => _MouseClickPosition = pos;

    private void StockUpdate(DateTime? startDate = null)
    {
        Update_CandleVMCollection_Stack();
        Queue<CandleViewModel> show = new();
        var count = GetNewCandleCount();
        while (count > 0 && _CandleVMCollection_Left!.Any()) {
            show.Enqueue(_CandleVMCollection_Left!.Pop());
            count--;
        }
        _CandleVMCollection = new();
        while (show.Any())
        {
            _CandleVMCollection.Add(show.Dequeue());
        }
        ResizeCandle();
    }
    private CandleViewModel CreateCandleVM(DateTime date, Price price)
    {
        ChartParameter cp = new()
        {
            Width = _CandleWidth,
            Height = CandleHeight,
            Highest = _HighestPrice,
            Lowest = _LowestPrice,
        };

        if (_TempLabelDate.Month != date.Month)
        {
            _TempLabelDate = date;
            return new CandleViewModel(date, price, cp, _HighestVolume, true);
        }

        return new CandleViewModel(date, price, cp, _HighestVolume);
    }
    private void Update_CandleVMCollection_Stack()
    {
        _CandleVMCollection_Right = new();
        _CandleVMCollection_Left = new();
        foreach (var entry in mStockModel.PriceData)
        {
            _CandleVMCollection_Left!.Push(CreateCandleVM(entry.Key,entry.Value));
        }
    }
    private int GetNewCandleCount() => (int)(_ChartSize.Width / (_CandleWidth + CandleMargin.Left + CandleMargin.Right)) + 1;
    public bool CandleSizeChanged() {
        var newCandleCount = GetNewCandleCount();
        var candleCount = _CandleVMCollection!.Count();
        if (newCandleCount > candleCount)
        {
            var addCount = newCandleCount - candleCount;
            if (AddCandleLeft(addCount))
                ResizeCandle();
            else
            {
                _ChartGridVM!.Resize(_ChartSize);
                return false;
            }
        }
        else if (newCandleCount < candleCount)
        {
            var reduceCount = candleCount - newCandleCount;
            if (ReduceCandleLeft(reduceCount))
                ResizeCandle();
            else
            {
                _ChartGridVM!.Resize(_ChartSize);
                return false;
            }
        }
        _ChartGridVM!.Resize(_ChartSize);
        ChartGridVM = _ChartGridVM;
        return true;

        bool ReduceCandleLeft(int reduceCount)
        {
            if (!_CandleVMCollection!.Any() || _CandleVMCollection!.Count() < reduceCount)
                return false;
            while (reduceCount > 0)
            {
                var candleVM = _CandleVMCollection!.Last();
                _CandleVMCollection_Left!.Push(candleVM);
                _CandleVMCollection!.RemoveAt(_CandleVMCollection!.Count() - 1);
                reduceCount--;
            }
            return true;
        }
        bool AddCandleLeft(int addCount)
        {
            if (!_CandleVMCollection_Left!.Any() || _CandleVMCollection_Left!.Count() < addCount)
                return false;
            while (addCount > 0)
            {
                var candleVM = _CandleVMCollection_Left!.Pop();

                _CandleVMCollection!.Add(candleVM);
                addCount--;
            }
            return true;
        }
    }
    private void ResizeChartGrid(double chartHeight)
    {
        _HighestPrice = _CandleVMCollection!.Max(x => x.Candle!.mPrice.mMax);
        _LowestPrice = _CandleVMCollection!.Min(x => x.Candle!.mPrice.mMin);
        int limitQuantity = (int)(chartHeight / ChartLineQuantityRatio);

        double priceInterval = _HighestPrice - _LowestPrice;
        int offset = 1;
        while (priceInterval / offset > limitQuantity)
        {
            offset *= 5;
        }
        _HighestPrice = _HighestPrice - _HighestPrice % offset + offset;
        _LowestPrice = _LowestPrice - _LowestPrice % offset;

        _ChartGridVM = new ChartGridViewModel(new ChartParameter() {
            Highest = _HighestPrice,
            Lowest = _LowestPrice,
            Width = _ChartSize.Width,
            Height = _ChartSize.Height,
        });
        ChartGridVM = _ChartGridVM;

    }
    private void ResizeCandle()
    {
        ResizeChartGrid(_ChartSize.Height);
        _HighestVolume = _CandleVMCollection!.Max(x => x.Candle!.mPrice.mVolume);
        foreach (var candleVm in _CandleVMCollection!)
        {
            candleVm.Resize(CandleHeight, _CandleWidth, _HighestPrice, _LowestPrice, _HighestVolume);
        }
        CandleVMCollection = _CandleVMCollection;
    }
    public void MouseDrag(Point pos)
    {
        if (!_CandleVMCollection_Left!.Any() && !_CandleVMCollection_Right!.Any())
            return;

        var count = GetCountFromCount(pos);
        if (count > 0)
            ChartMoveRight(count);
        else if (count < 0)
            ChartMoveLeft(-count);

        if(count != 0)
            _MouseClickPosition = pos;

        int GetCountFromCount(Point _pos) => (int)((_MouseClickPosition!.Value.X - _pos.X) / _CandleWidth);
        void ChartMoveLeft(int count) 
        {
            while (count > 0 && _CandleVMCollection_Left!.Any())
            {
                var candleVM = _CandleVMCollection_Left!.Pop();
                _CandleVMCollection!.Add(candleVM);

                candleVM = _CandleVMCollection!.First();
                _CandleVMCollection!.RemoveAt(0);
                _CandleVMCollection_Right!.Push(candleVM);
                count--;
            }
            ResizeCandle();
        }
        void ChartMoveRight(int count)
        {
            while (count > 0 && _CandleVMCollection_Right!.Any())
            {
                var candleVM = _CandleVMCollection_Right!.Pop();
                _CandleVMCollection!.Insert(0, candleVM);

                candleVM = _CandleVMCollection!.Last();
                _CandleVMCollection!.RemoveAt(_CandleVMCollection!.Count() - 1);
                _CandleVMCollection_Left!.Push(candleVM);
                count--;
            }
            ResizeCandle();
        }
    }

}
