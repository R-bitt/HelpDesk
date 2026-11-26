using HelpDesk.Models;

namespace HelpDesk.Helpers
{
    /// <summary>
    /// Текущий пользователь приложения. Данные хранятся только в памяти
    /// и очищаются при выходе из учётной записи.
    /// </summary>
    public static class UserSession
    {
        public static User? CurrentUser { get; private set; }

        public static void Start(User user)
        {
            CurrentUser = user;
        }

        public static void End()
        {
            CurrentUser = null;
        }
    }
}
