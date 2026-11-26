using System.Data.SqlClient;
using HelpDesk.Data;
using HelpDesk.Helpers;
using HelpDesk.Models;

namespace HelpDesk.Services
{
    /// <summary>
    /// Авторизация пользователя по таблицам Users и Roles.
    /// Окно отвечает только за интерфейс, проверка учётных данных — здесь.
    /// </summary>
    public class AuthService
    {
        public User? Authenticate(string login, string password)
        {
            const string sql = @"
                SELECT TOP (1)
                    u.Id,
                    u.Login,
                    u.FullName,
                    u.RoleId,
                    r.Name AS RoleName,
                    u.PasswordHash,
                    u.IsActive
                FROM dbo.Users AS u
                INNER JOIN dbo.Roles AS r ON r.Id = u.RoleId
                WHERE u.Login = @Login;";

            using (SqlConnection connection = Database.GetConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Login", System.Data.SqlDbType.NVarChar, 50).Value = login;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    if (!Convert.ToBoolean(reader["IsActive"]))
                    {
                        return null;
                    }

                    string storedHash = Convert.ToString(reader["PasswordHash"]) ?? string.Empty;

                    if (!PasswordHasher.VerifyPassword(password, storedHash))
                    {
                        return null;
                    }

                    return new User
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Login = Convert.ToString(reader["Login"]) ?? string.Empty,
                        FullName = Convert.ToString(reader["FullName"]) ?? string.Empty,
                        RoleId = Convert.ToInt32(reader["RoleId"]),
                        RoleName = Convert.ToString(reader["RoleName"]) ?? string.Empty,
                        IsActive = true
                    };
                }
            }
        }
    }
}
