namespace HelpDesk.Services
{
    /// <summary>
    /// Временная авторизация: логин и пароль заданы в коде.
    /// После подключения базы данных проверка будет выполняться по таблице Users.
    /// </summary>
    public class AuthService
    {
        public bool Authenticate(string login, string password)
        {
            return login == "admin" && password == "12345";
        }
    }
}
