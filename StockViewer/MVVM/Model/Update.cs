using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockViewer.MVVM.Model;
public static class Update
{
    private static readonly int mHours = 14;
    public static readonly DateTime Beginning = new(2004, 2, 11);
    // data8 before 2011/7/31, data9 since 2011/8/1
    public static readonly DateTime SwitchDay = new(2011, 7, 31);
    public static bool isBeforeSwitchDay(DateTime date) => date < SwitchDay;
    public static void SaveToLocalDatas(DateTime date, string dataPath) => date.SaveJson(dataPath, "Update");
    public static DateTime GetLocalLastUpdate(string dataPath)
    {
        DateTime result = FileManagement.LoadJson<DateTime>(dataPath, "Update");

        return result < Beginning ? Beginning : result;
    }
    public static DateTime CreateCatchTime(int year, int month, int day) => new DateTime(year, month, day, mHours, 0, 0);
}
