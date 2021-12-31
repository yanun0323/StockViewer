using System.Collections.ObjectModel;

namespace StockViewer.MVVM.ViewModel;

public class ChartGridViewModel
{
    public double Top { get; set; }
    public double X1 { get; set; }
    public double Y1 { get; set; }
    public double X2 { get; set; }
    public double Y2 { get; set; }

    public ChartGridViewModel(Size chart, double price, double highestPrice, double lowestPrice) 
    {
        Draw(chart, price, highestPrice, lowestPrice);
    }

    public void Resize(Size chart, double price, double highestPrice, double lowestPrice) => Draw(chart, price, highestPrice, lowestPrice);

    private void Draw(Size chart, double price, double highestPrice, double lowestPrice)
    {
        double ratio =  (highestPrice - lowestPrice) / chart.Height;
        double y = (highestPrice - price) * ratio;
        Top = (highestPrice - price) * ratio;
        X1 = 0;
        Y1 = 0;
        X2 = chart.Width;
        Y2 = 0;
    }
}
