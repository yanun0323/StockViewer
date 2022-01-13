namespace StockViewer.MVVM.ViewModel;
public class SubChartViewModel : ObservableObject
{
    IStockModel _StockModel;
    Size _Canvas = new(854, 77.5);
    Func<IBarParameter> _Func;
    IBarParameter _BarParam { get => _Func.Invoke(); }
    ObservableCollection<RectengleModel>? _Rectangles = new();

    public ICommand? MouseUpCommand { get; set; }
    public ICommand? MouseMoveCommand { get; set; }
    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    public ObservableCollection<RectengleModel>? Rectangles { get => _Rectangles; set { _Rectangles = value; OnPropertyChanged(); } }

    public SubChartViewModel(IStockModel stockModel, Func<IBarParameter> func)
    {
        _StockModel = stockModel;
        _Func = func;
        Refresh();

        MouseUpCommand = new RelayCommand<MouseButtonEventArgs>(e =>
        {
        });

        MouseMoveCommand = new RelayCommand<MouseEventArgs>(e =>
        {
        });

        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(args =>
        {
            _Canvas = args.NewSize;
            Refresh(args.NewSize);
        });
    }



    public void Update(IStockModel? stockModel = null)
    {
        if (stockModel != null)
            _StockModel = stockModel;
        Refresh();
    }



    void Refresh(Size? newSize = null)
    {
        if (newSize != null)
            _Canvas = newSize.Value;

        _Rectangles = null;
        _Rectangles = new();

        double width = _BarParam.Width;
        int index = _BarParam.Count;

        double max = _StockModel!.InstitutionData.Skip(_BarParam.Start).Take(index).Max(x => x.Value.mTrustSuper);
        double min = _StockModel!.InstitutionData.Skip(_BarParam.Start).Take(index).Min(x => x.Value.mTrustSuper);

        double ratio = _Canvas.Height / (max - min);
        List<Task> tasks = new();
        RectengleModel[] temp = new RectengleModel[index];
        foreach ((DateTime _, Institution instit) in _StockModel!.InstitutionData.Skip(_BarParam.Start))
        {
            if (--index < 0)
                break;

            if (instit.mTrustSuper == 0)
                continue;

            int i = index;
            Institution copy = instit;
            Task t = new(() => {
                temp[i] = (CreateThick(copy, i, max, min, width, ratio));
            });
            t.Start();
            tasks.Add(t);
        }
        Task.WhenAll(tasks).Wait();

        for (int i = 0; i < temp.Count(); i++)
        {
            _Rectangles.Add(temp[i]);
        }

        Rectangles = _Rectangles;

        static RectengleModel CreateThick(Institution instit, int i, double max, double min, double width, double ratio)
        {
            return new RectengleModel()
            {
                Width = width,
                Left = i * (width + 2),
                Top = ratio * (max - instit.mTrustSuper),
                Height = ratio * Math.Abs(instit.mTrustSuper),
                Color = instit.mTrustSuper > 0 ? iColor.Red : iColor.Green,
            };
        }
    }
}
