using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mApp.ViewModel
{
    public class MainChartViewModel : INotifyPropertyChanged
    {
        private Stock? _mStock { get; set; }
        public int Width { get => Width; set => OnPropertyChanged(nameof(Width)); }
        public int Height { get => Height; set => OnPropertyChanged(nameof(Height)); }

        //public Stock? mStock { get => _mStock; set { _mStock = mStock; OnPropertyChanged(nameof(mStock)); } }

        public event PropertyChangedEventHandler? PropertyChanged; 
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public MainChartViewModel(Stock iStock)
        {
            _mStock = iStock;
        }
    }

    public class Candle 
    {   

    }
}
