using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockViewer.MVVM.Model;
public class QuantityHash
{
    private SortedDictionary<double, HashSet<DateTime>> MaxHash = new();
    private SortedDictionary<double, HashSet<DateTime>> MinHash= new();

    public void Clear() 
    {
        MaxHash.Clear();
        MinHash.Clear();
    }
    public double GetMax() => MaxHash.Any() ? MaxHash.Last().Key : 0;
    public double GetMin() => MinHash.Any() ? MinHash.First().Key : 0;

    public void AddMax(double value, DateTime dateTime) 
    { 
        if(!MaxHash.ContainsKey(value))
            MaxHash.Add(value, new HashSet<DateTime>());
        MaxHash[value].Add(dateTime);
    }
    public void RemoveMax(double value, DateTime dateTime)
    {
        MaxHash[value].Remove(dateTime);
        if (MaxHash[value].Any())
            return;
        MaxHash.Remove(value);
    }
    public void AddMin(double value, DateTime dateTime)
    {
        if (!MinHash.ContainsKey(value))
            MinHash.Add(value, new HashSet<DateTime>());
        MinHash[value].Add(dateTime);
    }
    public void RemoveMin(double value, DateTime dateTime)
    {
        MinHash[value].Remove(dateTime);
        if (MinHash[value].Any())
            return;
        MinHash.Remove(value);
    }

}
