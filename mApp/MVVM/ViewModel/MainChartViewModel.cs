
using System.Collections.ObjectModel;
using System.Windows.Shapes;

namespace mApp.MVVM.ViewModel;
public class MainChartViewModel:ObservableObject
{
    public ICommand? MouseWheelCommand { get; set; }
    public ICommand? MouseUpCommand { get; set; }
    public ICommand? MouseMoveCommand { get; set; }
    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    public ICommand? CoordMouseMoveCommand { get; set; }


    private Grid? MainChartGrid;
    private Point? MouseClickPosition;

    private double _Top;
    private double _Bottom;
    private Stock? _mStock;
    private ObservableCollection<CandleViewModel>? _CandleVMCollection;
    private Stack<CandleViewModel>? _CandleVMCollection_Right;
    private Stack<CandleViewModel>? _CandleVMCollection_Left;
    private readonly Thickness _CandleMargin = new(1, 1, 1, 1);
    private Size _ChartSize = new(834, 305);
    private double _CandleWidth = 10;
    private double _CandleWidth_Max = 30;
    private double _CandleWidth_Min = 3;
    private DateTime _TempLabelDate;
    private int _HighestVolume;
    private bool _InfoPopShow = false;
    private CandleViewModel? _InfoVM;
    public string _InfoDate = "";

    public bool InfoPopShow { get => _InfoPopShow; set { _InfoPopShow = value; OnPropertyChanged(); } }
    public double _CandleHeight { get => _ChartSize.Height - _CandleMargin.Top - _CandleMargin.Bottom; }
    public double _CandleOutlineWidth { get => _CandleWidth + _CandleMargin.Left + _CandleMargin.Right; }
    public ObservableCollection<CandleViewModel>? CandleVMCollection { get => _CandleVMCollection; set { _CandleVMCollection = value; OnPropertyChanged(); } }
    public CandleViewModel? InfoVM { get => _InfoVM; set { _InfoVM = value; OnPropertyChanged(); } }
    public string InfoDate { get => _InfoDate; set { _InfoDate = value; OnPropertyChanged(); } }
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

        MouseUpCommand = new RelayCommand<MouseButtonEventArgs>(e => 
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                if (!InfoPopShow)
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
                InfoPopShow = !InfoPopShow;
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
            
            if (InfoPopShow)
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

    public void UpdateChart(Stock stock) => mStock = stock;

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

        if (_TempLabelDate.Month != date.Month)
        {
            _TempLabelDate = date;
            return new CandleViewModel(cp, _CandleMargin, true);
        }

        return new CandleViewModel(cp, _CandleMargin);
    }
    private void Update_CandleVMCollection_Stack()
    {
        _CandleVMCollection_Right = new();
        _CandleVMCollection_Left = new();
        foreach (var entry in mStock.TradingData)
        {
            _CandleVMCollection_Left!.Push(CreateCandleVM(entry.Key,entry.Value));
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
                _CandleVMCollection_Left!.Push(candleVM);
                _CandleVMCollection!.RemoveAt(_CandleVMCollection!.Count() - 1);
                reduceCount--;
            }
        }
        void AddCandleLeft(int addCount)
        {
            while (addCount > 0)
            {
                var candleVM = _CandleVMCollection_Left!.Pop();

                _CandleVMCollection!.Add(candleVM);
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
