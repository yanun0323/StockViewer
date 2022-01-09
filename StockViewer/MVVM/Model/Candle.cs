namespace StockViewer.MVVM.Model;
public class Candle
{
    public DateTime mDate { get; private set; }
    public Price mPrice { get; private set; }
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
    public ChartParameter Parameter { get; private set; }

    public Candle(DateTime dateTime, Price price, ChartParameter parameter) 
    {
        Update(dateTime, price, parameter);
    }

    public void Update(DateTime dateTime, Price price, ChartParameter parameter)
    {
        mDate = dateTime;
        mPrice = price;
        Parameter = parameter;
        Height = parameter.Height ;
        Width = parameter.Width;

        Ratio = (Height * CandleViewModel.CandleHeightRatio) / (parameter.Highest - parameter.Lowest);
        LineTop = (parameter.Highest - mPrice.mMax) * Ratio;
        LineHeight = Math.Abs((parameter.Highest - mPrice.mMin) * Ratio - LineTop);
        if (mPrice.mStart < mPrice.mEnd)
            BlockTop = (parameter.Highest - mPrice.mEnd) * Ratio;
        else
            BlockTop = (parameter.Highest - mPrice.mStart) * Ratio;

        BlockHeight = Math.Abs(mPrice.mEnd - mPrice.mStart) * Ratio;
        BlockHeight = (BlockHeight < 1) ? 1 : BlockHeight;
        LineLeft = (Width - LineWidth) / 2;
        mColor = (mPrice.End == mPrice.Start) ? iColor.Gray : (mPrice.mEnd - mPrice.mStart > 0) ? iColor.Red : iColor.Green;
    }

    public Candle Resize(double? height = null, double? width = null, double? top = null, double? bottom = null) {
        Parameter = new()
        {
            Highest = top ?? Parameter.Highest,
            Lowest = bottom ?? Parameter.Lowest,
            Width = width ?? Parameter.Width,
            Height = height ?? Parameter.Height
        };
        Update(mDate, mPrice, Parameter);
        return this;
    }

}
