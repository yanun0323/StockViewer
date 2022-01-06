
namespace StockViewer.Library;

public static class FilePath
{
    public static string Path_Raw_Root { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StockViewer\\RawData"); }
    public static string Path_Raw_Price { get => Path.Combine(Path_Raw_Root, "Price"); }
    public static string Name_Error_Price = "Price_Error";
    public static string Name_UpdateTime_Price = "Price_UpdateTime";
    public static string Path_Raw_Institution { get => Path.Combine(Path_Raw_Root, "Institution"); }
    public static string Name_Error_Institution = "Institution_Error";
    public static string Name_UpdateTimeName_Institution = "Institution_UpdateTime";

}
