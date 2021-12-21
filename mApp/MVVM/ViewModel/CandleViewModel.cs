namespace mApp.MVVM.ViewModel;
public class CandleViewModel : ObservableObject
{

    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    

    private Candle? _candle;
    private Thickness _candleMargin = new(2, 0, 2, 0);

    public Candle? Candle
    {
        get { return _candle; }
        set { _candle = value; OnPropertyChanged(); }
    }
    public Thickness CandleMargin
    {
        get { return _candleMargin; }
        set { _candleMargin = value; OnPropertyChanged(); }
    }

    public CandleViewModel(CandleParameter parameter)
    {
        Candle = new(parameter);

        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(Args => 
        {
            ResizeCandleHeight(Args.NewSize.Height);
        });

        LoadedCommand = new RelayCommand<Size>(size =>
        {
            ResizeCandleHeight(size.Height);
        });
    }
    public void Zoom(int delta, Size chartSize, int? candleCount) {
        double step = 1;
        double scale = (delta > 0) ? step : -step;
        double newWidth = (Candle!.Width + scale < 4) ? 4 : Candle!.Width + scale;
        ResizeCandleWidth(newWidth);
    }

    private void ResizeCandleHeight(double height) => Candle = Candle!.Resize_Height(height);
    private void ResizeCandleWidth(double width) => Candle = Candle!.Resize_Width(width);
}
