
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

    private Size _ChartSize = new(854, 361);
    private double _BarWidth = 10;
    private DateTime _TempLabelDate;
    private int _HighestVolume;
    private Visibility _InfoPopShow = Visibility.Hidden;
    private CandleViewModel? _InfoVM;
    private string _InfoDate = "";
    private ChartGridViewModel? _ChartGridVM;

    public static readonly double GridWidth = 50;
    public Visibility InfoPopShow { get => _InfoPopShow; set { _InfoPopShow = value; OnPropertyChanged(); } }
    public double CandleHeight { get => _ChartSize.Height; }
    public double BarWidth { get => _BarWidth; }
    public double BarMarginWidth { get => _BarWidth + CandleMargin.Left + CandleMargin.Right; }
    public ChartStructure<CandleViewModel> CandleVMStruct { get ; set; } 
        = new(new((CandleViewModel c) => (c.mPrice.mMax, c.mPrice.mMin, c.Date)));
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
            Refresh();
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
                        int index = (int)(distanceFromRight / BarMarginWidth);
                        InfoVM = CandleVMStruct.ElementAt(index);
                        InfoDate = $"{InfoVM!.Date:yyyy/MM/dd}";
                    }
                    else
                    {
                        InfoVM = CandleVMStruct.ElementAt(0);
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

                int index = (int)(distanceFromRight / BarMarginWidth);
                if (index >= CandleVMStruct.Count())
                    return;

                InfoVM = CandleVMStruct.ElementAt(index);
                InfoDate = $"{InfoVM!.Date:yyyy/MM/dd}";
            }
        });

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
            _MainChartGrid = obj;
        });
    }

    public void UpdateChart(StockModel stockModel) => mStockModel = stockModel;
    public void SetBarWidth(double width) => _BarWidth = width;
    public void SetMouseClickPosition(Point pos) => _MouseClickPosition = pos;
    public bool BarSizeChanged()
    {
        var newCandleCount = GetNewCandleCount();
        var candleCount = CandleVMStruct!.Count();
        if (newCandleCount > candleCount)
        {
            var addCount = newCandleCount - candleCount;
            CandleVMStruct.ZoomOut(addCount);
            ResizeBar();
        }
        else if (newCandleCount < candleCount)
        {
            var reduceCount = candleCount - newCandleCount;
            CandleVMStruct.ZoomIn(reduceCount);
            ResizeBar();
        }

        //ResizeBar();
        _ChartGridVM!.Resize(_ChartSize);
        ChartGridVM = _ChartGridVM;
        return true;
    }
    public void MouseDrag(Point pos)
    {
        if (CandleVMStruct.AllShow)
            return;

        var count = GetCountFromCount(pos);
        if (count > 0)
        {
            CandleVMStruct.PanRight(count);
            ResizeBar();
        }
        else if (count < 0) { 
            CandleVMStruct.PanLeft(-count);
            ResizeBar();
        }

        //ResizeBar();

        if (count != 0)
            _MouseClickPosition = pos;

        int GetCountFromCount(Point _pos) => (int)((_MouseClickPosition!.Value.X - _pos.X) / _BarWidth);
    }




    private int GetNewCandleCount() => (int)(_ChartSize.Width / _BarWidth) + 1;
    private void Refresh()
    {
        CandleVMStruct.Clear();
        foreach (var entry in mStockModel.PriceData)
        {
            CandleVMStruct.Push(CreateCandleVM(entry.Key, entry.Value));
        }
        Queue<CandleViewModel> show = new();
        int count = GetNewCandleCount();
        CandleVMStruct.Generate(count);
        Task.WhenAll().Wait();
        ResizeBar();
    }
    private CandleViewModel CreateCandleVM(DateTime date, Price price)
    {
        ChartParameter cp = new()
        {
            Width = _BarWidth,
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
    private void ResizeBar()
    {
        ResizeChartGrid(_ChartSize.Height);
        _HighestVolume = CandleVMStruct.GetMaxVolume(new((c) => c.mPrice.mVolume));

        //Task task = new(() =>
        //{
        //    foreach (var candleVm in CandleVMStruct.Middle)
        //    {
        //        candleVm.Resize(CandleHeight, _BarWidth, _HighestPrice, _LowestPrice, _HighestVolume);
        //    }
        //});
        //task.Start();
        //task.Wait();

        List<Task> tasks = new();
        foreach (var candleVm in CandleVMStruct.Middle)
        {
            var copy = candleVm;
            Task task = new(() =>
            {
                copy.Resize(CandleHeight, _BarWidth, _HighestPrice, _LowestPrice, _HighestVolume);
            });
            task.Start();
            tasks.Add(task);
        }
        Task.WhenAll(tasks).Wait();
        CandleVMStruct.Refresh();
    }
    private void ResizeChartGrid(double chartHeight)
    {
        _HighestPrice = CandleVMStruct.Max;
        _LowestPrice = CandleVMStruct.Min;
        int limitQuantity = (int)(chartHeight / ChartLineQuantityRatio);

        double priceInterval = _HighestPrice - _LowestPrice;
        double offset = 0.2;
        while (priceInterval / offset > limitQuantity)
        {
            offset *= 5;
        }
        _HighestPrice = _HighestPrice - _HighestPrice % offset + offset;
        _LowestPrice = _LowestPrice - _LowestPrice % offset;

        _ChartGridVM = new ChartGridViewModel(new ChartParameter()
        {
            Highest = _HighestPrice,
            Lowest = _LowestPrice,
            Width = _ChartSize.Width,
            Height = _ChartSize.Height,
        });
        ChartGridVM = _ChartGridVM;

    }
}
