
using System.Collections.ObjectModel;
using System.Windows.Shapes;

namespace StockViewer.MVVM.ViewModel;
public class MainChartViewModel:ObservableObject
{
    public ICommand? MouseWheelCommand { get; set; }
    public ICommand? MouseUpCommand { get; set; }
    public ICommand? MouseMoveCommand { get; set; }
    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    public ICommand? CoordMouseMoveCommand { get; set; }


    public static readonly Thickness CandleMargin = new(1, 0, 1, 0);
    private static readonly double _CandleWidth_Max = 30;
    private static readonly double _CandleWidth_Min = 3;

    private Grid? MainChartGrid;
    private Point? MouseClickPosition;

    private double _HighestPrice;
    private double _LowestPrice;
    private StockModel? _mStockModel;

    private ObservableCollection<CandleViewModel>? _CandleVMCollection;
    private Stack<CandleViewModel>? _CandleVMCollection_Right;
    private Stack<CandleViewModel>? _CandleVMCollection_Left;

    private Size _ChartSize = new(834, 305);
    private double _CandleWidth = 10;
    private DateTime _TempLabelDate;
    private int _HighestVolume;
    private Visibility _InfoPopShow = Visibility.Hidden;
    private CandleViewModel? _InfoVM;
    private string _InfoDate = "";
    private ChartGridViewModel? _ChartGridVM;

    public Visibility InfoPopShow { get => _InfoPopShow; set { _InfoPopShow = value; OnPropertyChanged(); } }
    public double _CandleHeight { get => _ChartSize.Height; }
    public double _CandleOutlineWidth { get => _CandleWidth + CandleMargin.Left + CandleMargin.Right; }
    public ObservableCollection<CandleViewModel>? CandleVMCollection { get => _CandleVMCollection; set { _CandleVMCollection = value; OnPropertyChanged(); } }
    public CandleViewModel? InfoVM { get => _InfoVM; set { _InfoVM = value; OnPropertyChanged(); } }
    public ChartGridViewModel? ChartGridVM { get => _ChartGridVM; set { _ChartGridVM = value; OnPropertyChanged(); } }
    public static readonly double GridWidth = 30;
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

        MouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(e =>
        {
            double step = (_CandleWidth > 20) ? 3 : 1;
            double scale = ((e.Delta > 0) ? 1 : -1) * step;
            if (_CandleWidth + scale > _CandleWidth_Min && _CandleWidth + scale < _CandleWidth_Max)
                _CandleWidth += scale;
            if (!CandleSizeChanged())
                _CandleWidth -= scale;
        });

        MouseUpCommand = new RelayCommand<MouseButtonEventArgs>(e => 
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                if (InfoPopShow != Visibility.Visible)
                {
                    Point pos = e.MouseDevice.GetPosition(MainChartGrid);
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
            if (e.LeftButton == MouseButtonState.Pressed && MouseClickPosition != null)
            {
                Point pos = e.MouseDevice.GetPosition(MainChartGrid);
                MouseDrag(pos);
            }
            else
            {
                MouseClickPosition = e.MouseDevice.GetPosition(MainChartGrid);
            }
            
            if (InfoPopShow == Visibility.Visible)
            {
                Point pos = e.MouseDevice.GetPosition(MainChartGrid);
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
            MainChartGrid = obj;
        });
    }

    public void UpdateChart(StockModel stockModel) => mStockModel = stockModel;

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
        CandleParameter cp = new()
        {
            Width = _CandleWidth,
            Height = _CandleHeight,
            Top = _HighestPrice,
            Bottom = _LowestPrice,
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
    private bool CandleSizeChanged() {
        var newCandleCount = GetNewCandleCount();
        var candleCount = _CandleVMCollection!.Count();
        if (newCandleCount > candleCount)
        {
            var addCount = newCandleCount - candleCount;
            if (AddCandleLeft(addCount))
                ResizeCandle();
            else
                return false;
        }
        else if (newCandleCount < candleCount)
        {
            var reduceCount = candleCount - newCandleCount;
            if (ReduceCandleLeft(reduceCount))
                ResizeCandle();
            else
                return false;
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
    private void ResizeCandle()
    {
        _HighestPrice = _CandleVMCollection!.Max(x => x.Candle!.mPrice.mMax);
        _LowestPrice = _CandleVMCollection!.Min(x => x.Candle!.mPrice.mMin);

        double priceInterval = _HighestPrice - _LowestPrice;
        int offset = 1;
        while (priceInterval / offset > 15)
        {
            offset *= 5;
        }
        _HighestPrice = _HighestPrice - _HighestPrice % offset + offset;
        _LowestPrice = _LowestPrice - _LowestPrice % offset;

        _HighestVolume = _CandleVMCollection!.Max(x => x.Candle!.mPrice.mVolume);

        foreach (var candleVm in _CandleVMCollection!)
        {
            candleVm.Resize(_CandleHeight, _CandleWidth, _HighestPrice, _LowestPrice, _HighestVolume);
        }
        _ChartGridVM = new(new(_ChartSize.Width + 50.0, _ChartSize.Height), _HighestPrice, _LowestPrice);

        ChartGridVM = _ChartGridVM;
        CandleVMCollection = _CandleVMCollection;
    }
    private void MouseDrag(Point pos)
    {
        if (!_CandleVMCollection_Left!.Any() && !_CandleVMCollection_Right!.Any())
            return;

        var count = GetCountFromCount(pos);
        if (count > 0)
            ChartMoveRight(count);
        else if (count < 0)
            ChartMoveLeft(-count);

        if(count != 0)
            MouseClickPosition = pos;

        int GetCountFromCount(Point _pos) => (int)((MouseClickPosition!.Value.X - _pos.X) / (_CandleWidth + CandleMargin.Left + CandleMargin.Right));
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
