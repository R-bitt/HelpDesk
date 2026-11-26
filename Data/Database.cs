using System.Data.SqlClient;

namespace HelpDesk.Data
{
    /// <summary>
    /// Единая точка подключения к базе данных HelpDesk.
    /// Строка подключения хранится в одном месте, чтобы её было удобно изменить.
    /// </summary>
    public static class Database
    {
        public static string ConnectionString =
            @"Server=localhost;Database=HelpDesk_Chetverikov;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
