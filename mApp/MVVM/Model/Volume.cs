namespace mApp.MVVM.Model;
public class Volume
{
    public DateTime Date { get; private set; }
    public double Height { get; private set; }
    public double BlockHeight { get; private set; }

    private double mVolume;
    private double VolumeRatio;
    private double HeightRatio;
    private readonly double BlockHeight_Min = 2;

    public Volume(CandleParameter parameter, double candleHeightRatio)
    {
        HeightRatio = candleHeightRatio;
        Update(parameter);
    }

    public void Update(CandleParameter parameter)
    {
        Date = parameter.Date;
        mVolume = parameter.Tr.mVolume;
        CalculateBlock(parameter.Height, parameter.HighestVolume);
    }

    public Volume Resize(double? height = null, double? highestVolume = null)
    {
        if (height == null)
            return this;

        CalculateBlock(height.Value, highestVolume);
        return this;
    }

    private void CalculateBlock(double height, double? highestVolume = null)
    {
        if(highestVolume != null) 
             VolumeRatio = mVolume / highestVolume.Value;
        Height = height * HeightRatio;
        BlockHeight = Height * VolumeRatio;
        if (BlockHeight < BlockHeight_Min)
            BlockHeight = BlockHeight_Min;
    }

}
