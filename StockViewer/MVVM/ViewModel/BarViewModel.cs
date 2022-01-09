namespace StockViewer.MVVM.ViewModel;
public class BarViewModel : ObservableObject
{
    public double RectTop { get => _RectTop; private set { _RectTop = value; OnPropertyChanged(); } }
    public double RectWidth { get => _RectWidth; private set { _RectWidth = value; OnPropertyChanged(); } }
    public double RectHeight { get => _RectHeight; private set { _RectHeight = value; OnPropertyChanged(); }}
    public double Ratio { get; private set; }
    public DateTime mDate { get; private set; }
    public Institution Insti { get => _Insti; private set { _Insti = value; OnPropertyChanged(); } }
    public ChartParameter Parameter { get => _Parameter; private set { _Parameter = value; OnPropertyChanged(); } }
    public SolidColorBrush? mColor { get; private set; }
    public Thickness Margin { get => MainChartViewModel.CandleMargin; }

    private double _RectTop;
    private double _RectWidth;
    private double _RectHeight;
    private InstitutionOption Option;
    private Institution _Insti;
    private ChartParameter _Parameter;

    public BarViewModel(DateTime date, Institution insti, ChartParameter Param, InstitutionOption option) 
    {
        Draw(date, insti, Param, option);
    }

    private void Draw(DateTime date, Institution insti, ChartParameter Param, InstitutionOption option)
    {
        mDate = date;
        Insti = insti;
        Parameter = new()
        {
            Highest = Param.Highest,
            Lowest = Param.Lowest,
            Width = Param.Width - Margin.Left - Margin.Right,
            Height = Param.Height
        };
        Option = option;
        Ratio = (Parameter.Height) / (Parameter.Highest - Parameter.Lowest);

        RectTop = Insti.mTrustSuper > 0 ? (Parameter.Highest - Insti.mTrustSuper) * Ratio : Parameter.Highest * Ratio;
        RectHeight = Insti.mTrustSuper > 0 ? Insti.mTrustSuper * Ratio : -Insti.mTrustSuper * Ratio;
        RectWidth = Param.Width - Margin.Left - Margin.Right;
        mColor = Insti.mTrustSuper > 0 ? iColor.Red : Insti.mTrustSuper < 0 ? iColor.Green : iColor.Transparent;
    }
    public void Resize(double? height = null, double? width = null, double? top = null, double? bottom = null, InstitutionOption? option = null)
    {
        Parameter = new()
        {
            Highest = top ?? Parameter.Highest,
            Lowest = bottom ?? Parameter.Lowest,
            Width = width ?? Parameter.Width,
            Height = height ?? Parameter.Height,
        };
        Option = option ?? Option;
        Draw(mDate, Insti, Parameter, Option);
    }
}
