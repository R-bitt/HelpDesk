namespace HelpDesk.Data
{
    /// <summary>
    /// Класс доступа к базе данных MS SQL Server.
    /// На текущем этапе хранит строку подключения;
    /// запросы ADO.NET будут добавлены в следующих практических работах.
    /// </summary>
    public class Database
    {
        private const string ConnectionString =
            @"Server=localhost;Database=HelpDesk;Trusted_Connection=True;TrustServerCertificate=True;";

        public string GetConnectionString()
        {
            return ConnectionString;
        }
    }
}
