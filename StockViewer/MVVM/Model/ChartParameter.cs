using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockViewer.MVVM.Model; 

public struct ChartParameter
{
    public double Highest { get; set; }
    public double Lowest { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}
