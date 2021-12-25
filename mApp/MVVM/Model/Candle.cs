namespace mApp.MVVM.Model;
public class Candle
{
    public DateTime Date { get; private set; }
    public SolidColorBrush? mColor { get; private set; }
    public double Height { get; private set; }
    public double Width { get; private set; }
    public double Ratio { get; private set; }
    public double LineTop { get; private set; }
    public double LineHeght { get; private set; }
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
        Date = parameter.Date;
        Height = parameter.Height * HeightRatio;
        Width = parameter.Width;

        Ratio = Height / (parameter.Top - parameter.Bottom);
        LineTop = (parameter.Top - parameter.Tr.mMax) * Ratio;
        LineHeght = Math.Abs((parameter.Top - parameter.Tr.mMin) * Ratio - LineTop);
        if (parameter.Tr.mStart < parameter.Tr.mEnd)
        {
            BlockTop = (parameter.Top - parameter.Tr.mEnd) * Ratio;
            BlockHeight = Math.Abs((parameter.Top - parameter.Tr.mStart) * Ratio - BlockTop);
        }
        else
        {
            BlockTop = (parameter.Top - parameter.Tr.mStart) * Ratio;
            BlockHeight = Math.Abs((parameter.Top - parameter.Tr.mEnd) * Ratio - BlockTop);
        }
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
    public double HighestVolume { get; set; }
}
