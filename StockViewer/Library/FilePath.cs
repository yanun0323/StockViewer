
namespace StockViewer.Library;

public static class FilePath
{
    public static string Path_APP { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StockViewer"); }
    public static string Path_Data { get => Path.Combine(Path_APP, "Data"); }
    public static string Path_Stock { get => Path.Combine(Path_Data, "Stock"); }
    public static string Path_Raw_Root { get => Path.Combine(Path_APP, "RawData"); }
    public static string Path_Raw_Price { get => Path.Combine(Path_Raw_Root, "Price"); }
    public static string Path_Raw_Institution { get => Path.Combine(Path_Raw_Root, "Institution"); }
    public static string Path_StockList { get => Path_Data; }
    public static string Path_MySQL { get => Path_APP; }


    public static string Name_MySQL = "MySQL";
    public static string Name_Error_Price = "Price_Error";
    public static string Name_UpdateTime_Price = "Price_UpdateTime";
    public static string Name_Error_Institution = "Institution_Error";
    public static string Name_UpdateTimeName_Institution = "Institution_UpdateTime";
    public static string Name_UpdateTime = "UpdateTime";
    public static string Name_StockList = "StockList";

}
