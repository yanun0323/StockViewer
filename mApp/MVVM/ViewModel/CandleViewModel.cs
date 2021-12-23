namespace mApp.MVVM.ViewModel;
public class CandleViewModel : ObservableObject
{

    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    

    private Candle? _Candle;
    private Volume? _Volume;
    private Thickness _CandleMargin;

    public Candle? Candle
    {
        get { return _Candle; }
        set { _Candle = value; OnPropertyChanged(); }
    }
    public Thickness CandleMargin
    {
        get { return _CandleMargin; }
        set { _CandleMargin = value; OnPropertyChanged(); }
    }

    public CandleViewModel(CandleParameter parameter, Thickness candleMargin)
    {
        Candle = new(parameter);
        _CandleMargin = candleMargin;

        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(Args => 
        {
            ResizeCandleHeight(Args.NewSize.Height);
        });

        LoadedCommand = new RelayCommand<Size>(size =>
        {
            ResizeCandleHeight(size.Height);
        });
    }
    public void Zoom(int delta, Size chartSize) {
        double step = 1;
        double scale = ((delta > 0) ? 1 : -1) * step;
        double newWidth = (Candle!.Width + scale < 4) ? 4 : Candle!.Width + scale;
        ResizeCandleWidth(newWidth);
    }

    public void ResizeCandleHeight(double height) => Candle = Candle!.Resize_Height(height);
    public void ResizeCandleWidth(double width) => Candle = Candle!.Resize_Width(width);
}
