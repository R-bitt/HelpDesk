/*
    Практическая работа №11.
    Настройка демонстрационных паролей HelpDesk.

    В таблицу Users записываются не пароли, а PBKDF2-хеши
    с индивидуальной случайной солью. Формат:
        итерации.соль(Base64).хеш(Base64)

    Учебные учётные записи:
        admin    / Admin123!    — Администратор
        support1 / Support123!  — Специалист
        user1    / User123!     — Пользователь

    Эти пароли предназначены только для локальной учебной базы.
    Перед выполнением замените HelpDesk_Chetverikov на имя своей базы,
    если оно отличается.
*/

USE HelpDesk_Chetverikov;
GO

UPDATE dbo.Users
SET PasswordHash = N'100000.xeMqSzsIVsEi8+WqYJgb5A==.XRHMOiQZklu4UMlTVMkEwdTaX6TYC+51DkcnQ4zIhOw='
WHERE Login = N'admin';
GO

UPDATE dbo.Users
SET PasswordHash = N'100000.t1+4bRZ84HkHgCR17Aaghg==.OPVy7qfq8c68LofqdL5S5DH94/ouNE8+1lSWx9DTOb8='
WHERE Login = N'support1';
GO

UPDATE dbo.Users
SET PasswordHash = N'100000.ivj8bMVgRI3IHxq2lp3oCA==.NlD8mV4ABWpqYXJaMmgRfoRTtY1m2oavnU0V+5YkDwA='
WHERE Login = N'user1';
GO

SELECT
    u.Id,
    u.Login,
    u.FullName,
    r.Name AS RoleName,
    u.IsActive,
    CASE WHEN u.PasswordHash LIKE N'100000.%' THEN N'PBKDF2' ELSE N'НЕ ХЕШ' END AS HashType
FROM dbo.Users AS u
INNER JOIN dbo.Roles AS r ON r.Id = u.RoleId
ORDER BY u.Id;
GO
