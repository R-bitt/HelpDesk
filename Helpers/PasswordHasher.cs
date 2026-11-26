using System;
using System.Security.Cryptography;

namespace HelpDesk.Helpers
{
    /// <summary>
    /// Хеширование и проверка паролей по алгоритму PBKDF2.
    /// В базе хранится строка вида: итерации.соль(Base64).хеш(Base64).
    /// </summary>
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA1,
                HashSize);

            return Iterations + "." +
                   Convert.ToBase64String(salt) + "." +
                   Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Проверяет введённый пароль по сохранённому хешу.
        /// Пароль из хеша не восстанавливается.
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
            {
                return false;
            }

            string[] parts = storedHash.Split('.');
            if (parts.Length != 3)
            {
                return false;
            }

            if (!int.TryParse(parts[0], out int iterations) ||
                iterations < 10000 || iterations > 1000000)
            {
                return false;
            }

            try
            {
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] expectedHash = Convert.FromBase64String(parts[2]);

                if (salt.Length == 0 || expectedHash.Length == 0)
                {
                    return false;
                }

                byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA1,
                    expectedHash.Length);

                if (actualHash.Length != expectedHash.Length)
                {
                    return false;
                }

                int difference = 0;
                for (int i = 0; i < actualHash.Length; i++)
                {
                    difference |= actualHash[i] ^ expectedHash[i];
                }

                return difference == 0;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
