namespace HelpDesk.Models
{
    /// <summary>
    /// Сведения о вошедшем пользователе.
    /// Хеш пароля в объект не передаётся: он нужен только AuthService.
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
