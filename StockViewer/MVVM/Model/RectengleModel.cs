using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockViewer.MVVM.Model;
public class RectengleModel : ObservableObject
{
    double _Top;
    double _Left;
    double _Width;
    double _Height;
    SolidColorBrush _Color = iColor.Gray;
    public double Top { get => _Top; set { _Top = value; OnPropertyChanged(); } }
    public double Left { get => _Left; set { _Left = value; OnPropertyChanged(); } }
    public double Width { get => _Width; set { _Width = value; OnPropertyChanged(); } }
    public double Height { get => _Height; set { _Height = value; OnPropertyChanged(); } }
    public SolidColorBrush Color { get => _Color; set { _Color = value; OnPropertyChanged(); } }
}
