
using System.Collections.ObjectModel;

namespace mApp.MVVM.ViewModel;
public class MainChartViewModel:ObservableObject
{
    public ICommand? MouseWheelCommand { get; set; }
    public ICommand? SizeChangedCommand { get; set; }


    private double _Top;
    private double _Bottom;
    private Stack<(DateTime, TradingData)>? _RightDateStack;
    private Stack<(DateTime, TradingData)>? _LeftDateStack;
    private Stock? _mStock;
    private ObservableCollection<CandleViewModel>? _CandleVMCollection;
    private readonly Thickness _CandleMargin = new(2, 2, 2, 2);
    private Size _ChartSize = new(834, 305);
    private double _CandleWidth = 10;
    private double _CandleWidth_Max = 30;
    private double _CandleWidth_Min = 4;
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
            double step = 1;
            double scale = ((e.Delta > 0) ? 1 : -1) * step;
            if (_CandleWidth + scale > _CandleWidth_Min && _CandleWidth + scale < _CandleWidth_Max)
                _CandleWidth += scale;
            CandleSizeChanged();
        });

        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(args =>
        {
            _ChartSize = args.NewSize;
            CandleSizeChanged();
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
            Tr = data
        };
        return new CandleViewModel(cp, _CandleMargin);
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
            while (addCount > 0)
            {
                var (date, data) = _LeftDateStack!.Pop();

                if (data.mMax > _Top) _Top = data.mMax;
                if (data.mMin < _Bottom) _Bottom = data.mMin;

                _CandleVMCollection!.Add(CreateCandleVM(date, data));
                addCount--;
            }
            CandleVMCollection = _CandleVMCollection;
            ResizeCandle();
        }
        else if (newCandleCount < _CandleVMCollection!.Count())
        {
            var reduceCount =  candleCount - newCandleCount;
            while (reduceCount > 0)
            {
                var candleVM = _CandleVMCollection!.Last();
                _LeftDateStack!.Push(new(candleVM!.Candle!.Date, candleVM!.Candle.Parameter.Tr));
                _CandleVMCollection!.Remove(candleVM);
                reduceCount--;
            }
            CandleVMCollection = _CandleVMCollection;
            _Top = CandleVMCollection!.Max(x => x.Candle!.Parameter.Tr.mMax);
            _Bottom = CandleVMCollection!.Min(x => x.Candle!.Parameter.Tr.mMin);
            ResizeCandle();
        }
    }
    private void ResizeCandle()
    {
        foreach (var candleVm in _CandleVMCollection!)
        {
            candleVm.Resize(_CandleHeight, _CandleWidth, _Top, _Bottom);
        }

    }

}
