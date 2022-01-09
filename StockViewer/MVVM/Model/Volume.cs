namespace StockViewer.MVVM.Model;
public class Volume
{
    public DateTime Date { get; private set; }
    public double Height { get; private set; }
    public double BlockHeight { get; private set; }

    private int mVolume;
    private double VolumeRatio;
    private readonly double BlockHeight_Min = 2;

    public Volume(DateTime dateTime, Price price, ChartParameter parameter, int highestVolume)
    {
        Update(dateTime, price, parameter, highestVolume);
    }

    public void Update(DateTime dateTime, Price price, ChartParameter parameter, int highestVolume)
    {
        Date = dateTime;
        mVolume = price.mVolume;
        CalculateBlock(parameter.Height, highestVolume);
    }

    public Volume Resize(double? height = null, int? highestVolume = null)
    {
        if (height == null)
            return this;

        CalculateBlock(height.Value, highestVolume);
        return this;
    }

    private void CalculateBlock(double height, int? highestVolume = null)
    {
        if(highestVolume != null) 
             VolumeRatio = mVolume / (double)highestVolume.Value;
        Height = height * CandleViewModel.VolumeHeightRatio;
        BlockHeight = Height * VolumeRatio;
        if (BlockHeight < BlockHeight_Min)
            BlockHeight = BlockHeight_Min;
    }

}
