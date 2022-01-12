using MySql.Data.MySqlClient;

namespace StockViewer.Library;
public static class Server
{
    public static MySqlConnection? Connect()
    {
        string data = FileManagement.LoadText(FilePath.Path_MySQL, FilePath.Name_MySQL) ?? "_";
        string[] info = data.Split("\r\n");
        if (data == "_")
            return null;

        MySqlConnectionStringBuilder builder = new()
        {
            Server = info[0],
            UserID = info[1],
            Password = info[2],
            Port = uint.Parse(info[3]),
            //Database = info[4],
            SslMode = MySqlSslMode.None,

        };

        MySqlConnection conn = new (builder.ConnectionString);
        try
        {
            conn.Open();
            Trace.WriteLine("已經建立連線");
        }
        catch (MySqlException ex)
        {
            Trace.WriteLine(ex.Message);
        }
        finally
        {
            //conn.Close();
        }
        return conn;
    }
    public static void Close(MySqlConnection conn) => conn.Close();
}
