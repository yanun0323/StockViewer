using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockViewer.Library;
public static class Extention
{
    private static readonly DateTime _StockBegin = new(2004, 2, 11);
    public static DateTime GetStockBegin(this DateTime dateTime) => _StockBegin;
    public static DateTime GetStockSwitch(this DateTime dateTime) => new (2011, 7, 31);
    // data8 before 2011/7/31, data9 since 2011/8/1
    public static bool IsBeforeSwitchDay(this DateTime dateTime) => dateTime != new DateTime(2006, 09, 29) && dateTime < dateTime.GetStockSwitch();
    public static void Save(this DateTime date, string path) => date.SaveJson(path, FilePath.Name_UpdateTime);
}
