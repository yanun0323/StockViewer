namespace StockViewer.MVVM.Model;
public class Candle
{
    public SolidColorBrush? mColor { get; private set; }
    public double Height { get; private set; }
    public double Width { get; private set; }
    public double Ratio { get; private set; }
    public double LineTop { get; private set; }
    public double LineHeight { get; private set; }
    public double BlockTop { get; private set; }
    public double BlockHeight { get; private set; }
    public int LineWidth { get; } = 1;
    public double LineLeft { get; private set; }
    public CandleParameter Parameter { get; private set; }

    private double HeightRatio;

    public Candle(CandleParameter parameter, double candleHeightRatio) 
    {
        HeightRatio = candleHeightRatio;
        Update(parameter);
    }

    public void Update(CandleParameter parameter)
    {
        Parameter = parameter;
        Height = parameter.Height ;
        Width = parameter.Width;

        Ratio = (Height * HeightRatio) / (parameter.Top - parameter.Bottom);
        LineTop = (parameter.Top - parameter.Tr.mMax) * Ratio;
        LineHeight = Math.Abs((parameter.Top - parameter.Tr.mMin) * Ratio - LineTop);
        if (parameter.Tr.mStart < parameter.Tr.mEnd)
            BlockTop = (parameter.Top - parameter.Tr.mEnd) * Ratio;
        else
            BlockTop = (parameter.Top - parameter.Tr.mStart) * Ratio;

        BlockHeight = Math.Abs(parameter.Tr.mEnd - parameter.Tr.mStart) * Ratio;
        BlockHeight = (BlockHeight < 2) ? 2 : BlockHeight;
        LineLeft = (Width - LineWidth) / 2;
        mColor = (parameter.Tr.End == parameter.Tr.Start) ? iColor.Gray : (parameter.Tr.mEnd - parameter.Tr.mStart > 0) ? iColor.Red : iColor.Green;
    }

    public Candle Resize(double? height = null, double? width = null, double? top = null, double? bottom = null) {
        Parameter = new()
        {
            Date = Parameter.Date,
            Tr = Parameter.Tr,
            Top = top ?? Parameter.Top,
            Bottom = bottom ?? Parameter.Bottom,
            Width = width ?? Parameter.Width,
            Height = height ?? Parameter.Height
        };
        Update(Parameter);
        return this;
    }

}

public struct CandleParameter {
    public DateTime Date { get; set; }
    public TradingData Tr { get; set; }
    public double Top { get; set; }
    public double Bottom { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public int HighestVolume { get; set; }
}
