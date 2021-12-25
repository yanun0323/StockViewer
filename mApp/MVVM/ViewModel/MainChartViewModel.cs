
using System.Collections.ObjectModel;

namespace mApp.MVVM.ViewModel;
public class MainChartViewModel:ObservableObject
{
    public ICommand? MouseWheelCommand { get; set; }
    public ICommand? MouseLDownCommand { get; set; }
    public ICommand? MouseLUpCommand { get; set; }
    public ICommand? MouseMoveCommand { get; set; }
    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }


    private Grid? MainChartGrid;
    private Point? MouseClickPosition;

    private double _Top;
    private double _Bottom;
    private Stack<(DateTime, TradingData)>? _RightDateStack;
    private Stack<(DateTime, TradingData)>? _LeftDateStack;
    private Stock? _mStock;
    private ObservableCollection<CandleViewModel>? _CandleVMCollection;
    private readonly Thickness _CandleMargin = new(1, 1, 1, 1);
    private Size _ChartSize = new(834, 305);
    private double _CandleWidth = 10;
    private double _CandleWidth_Max = 30;
    private double _CandleWidth_Min = 3;

    private double _HighestVolume;
    public double _CandleHeight { get => _ChartSize.Height - _CandleMargin.Top - _CandleMargin.Bottom; }

    public ObservableCollection<CandleViewModel>? CandleVMCollection { get => _CandleVMCollection; set { _CandleVMCollection = value; OnPropertyChanged(); } }

    public Stock mStock
    {
        get => _mStock ?? new();
        set 
        { 
            _mStock = value;
            StockUpdate();
            OnPropertyChanged();
        }
    }

    public MainChartViewModel(Stock stock)
    {
        mStock = stock;

        MouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(e =>
        {
            double step = _CandleWidth / 4;
            double scale = ((e.Delta > 0) ? 1 : -1) * step;
            if (_CandleWidth + scale > _CandleWidth_Min && _CandleWidth + scale < _CandleWidth_Max)
                _CandleWidth += scale;
            CandleSizeChanged();
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
        });

        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(args =>
        {
            _ChartSize = args.NewSize;
            CandleSizeChanged();
        });

        LoadedCommand = new RelayCommand<Grid>(obj =>
        {
            MainChartGrid = obj;
        });
    }

    public void UpdateChart(Stock stock) => mStock = stock;

    private void StockUpdate(DateTime? startDate = null)
    {
        Update_DateStack();
        Queue<(DateTime,TradingData)> show = new();
        var count = GetNewCandleCount();
        while (count > 0) {
            var entry = _LeftDateStack!.Pop();
            show.Enqueue(new(entry.Item1, entry.Item2));
            count--;
        }
        _Top = show.Max(x => x.Item2.mMax);
        _Bottom = show.Min(x => x.Item2.mMin);
        _HighestVolume = show.Max(x => x.Item2.mVolume);

        _CandleVMCollection = new();
        while (show.Any())
        {
            var (date, data)= show.Dequeue();
            _CandleVMCollection.Add(CreateCandleVM(date, data));
        }
        CandleVMCollection = _CandleVMCollection;
    }
    private CandleViewModel CreateCandleVM(DateTime date, TradingData data)
    {
        CandleParameter cp = new()
        {
            Date = date,
            Width = _CandleWidth,
            Height = _CandleHeight,
            Top = _Top,
            Bottom = _Bottom,
            Tr = data,
            HighestVolume = _HighestVolume,
        };
        return new CandleViewModel(cp, _CandleMargin, _HighestVolume);
    }
    private void Update_DateStack()
    {
        _RightDateStack = new();
        _LeftDateStack = new();
        foreach (var entry in mStock.TradingData.Reverse())
        {
            _LeftDateStack!.Push(new(entry.Key,entry.Value));
        }
    }
    private int GetNewCandleCount() => (int)(_ChartSize.Width / (_CandleWidth + _CandleMargin.Left + _CandleMargin.Right)) + 1;
    private void CandleSizeChanged() {
        var newCandleCount = GetNewCandleCount();
        var candleCount = _CandleVMCollection!.Count();
        if (newCandleCount > _CandleVMCollection!.Count())
        {
            var addCount = newCandleCount - candleCount;
            AddCandleLeft(addCount);
            ResizeCandle();
        }
        else if (newCandleCount < _CandleVMCollection!.Count())
        {
            var reduceCount = candleCount - newCandleCount;
            ReduceCandleLeft(reduceCount);
            ResizeCandle();
        }

        void ReduceCandleLeft(int reduceCount)
        {
            while (reduceCount > 0)
            {
                var candleVM = _CandleVMCollection!.Last();
                _LeftDateStack!.Push(new(candleVM!.Candle!.Date, candleVM!.Candle.Parameter.Tr));
                _CandleVMCollection!.RemoveAt(_CandleVMCollection!.Count() - 1);
                reduceCount--;
            }
        }
        void AddCandleLeft(int addCount)
        {
            while (addCount > 0)
            {
                var (date, data) = _LeftDateStack!.Pop();

                _CandleVMCollection!.Add(CreateCandleVM(date, data));
                addCount--;
            }
        }
    }
    private void ResizeCandle()
    {
        _Top = _CandleVMCollection!.Max(x => x.Candle!.Parameter.Tr.mMax);
        _Bottom = _CandleVMCollection!.Min(x => x.Candle!.Parameter.Tr.mMin);
        _HighestVolume = _CandleVMCollection!.Max(x => x.Candle!.Parameter.Tr.mVolume);
        foreach (var candleVm in _CandleVMCollection!)
        {
            candleVm.Resize(_CandleHeight, _CandleWidth, _Top, _Bottom, _HighestVolume);
        }
        CandleVMCollection = _CandleVMCollection;
    }
    private void MouseDrag(Point pos)
    {
        var count = GetCountFromCount(pos);
        if (count > 0)
            ChartMoveRight(count);
        else if (count < 0)
            ChartMoveLeft(-count);

        if(count != 0)
            MouseClickPosition = pos;

        int GetCountFromCount(Point _pos) => (int)((MouseClickPosition!.Value.X - _pos.X) / (_CandleWidth + _CandleMargin.Left + _CandleMargin.Right));
        void ChartMoveLeft(int count) 
        {
            while (count > 0 && _LeftDateStack!.Any())
            {
                var (date, data) = _LeftDateStack!.Pop();
                _CandleVMCollection!.Add(CreateCandleVM(date, data));

                var candleVM = _CandleVMCollection!.First();
                _CandleVMCollection!.RemoveAt(0);
                _RightDateStack!.Push(new(candleVM!.Candle!.Date, candleVM!.Candle.Parameter.Tr));
                count--;
            }
            ResizeCandle();
        }
        void ChartMoveRight(int count)
        {
            while (count > 0 && _RightDateStack!.Any())
            {
                var (date, data) = _RightDateStack!.Pop();
                _CandleVMCollection!.Insert(0, CreateCandleVM(date, data));

                var candleVM = _CandleVMCollection!.Last();
                _CandleVMCollection!.RemoveAt(_CandleVMCollection!.Count() - 1);
                _LeftDateStack!.Push(new(candleVM!.Candle!.Date, candleVM!.Candle.Parameter.Tr));
                count--;
            }
            ResizeCandle();
        }
    }

}
