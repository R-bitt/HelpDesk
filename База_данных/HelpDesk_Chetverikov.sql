/*
    Практическая работа №9.
    База данных HelpDesk_Chetverikov для MS SQL Server.

    Скрипт создаёт базу данных, шесть таблиц, связи и тестовые данные.
    Пароли пользователей на этом этапе — учебные заглушки TEMP_*.
    Реальные хеши паролей записываются в Практической работе №11.

    Запуск: SSMS или
    sqlcmd -S localhost -E -C -f 65001 -i HelpDesk_Chetverikov.sql
*/

USE master;
GO

IF DB_ID(N'HelpDesk_Chetverikov') IS NULL
BEGIN
    CREATE DATABASE HelpDesk_Chetverikov;
END;
GO

USE HelpDesk_Chetverikov;
GO

/* ---------- Справочники ---------- */

IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        Id   INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL UNIQUE
    );
END;
GO

IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories
    (
        Id   INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL UNIQUE
    );
END;
GO

IF OBJECT_ID(N'dbo.Priorities', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Priorities
    (
        Id        INT IDENTITY(1,1) PRIMARY KEY,
        Name      NVARCHAR(50) NOT NULL UNIQUE,
        SortOrder INT NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.Statuses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Statuses
    (
        Id   INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL UNIQUE
    );
END;
GO

/* ---------- Пользователи ---------- */

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        Login        NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        FullName     NVARCHAR(150) NOT NULL,
        RoleId       INT NOT NULL,
        IsActive     BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK_Users_Roles
            FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id)
    );
END;
GO

/* ---------- Заявки ---------- */

IF OBJECT_ID(N'dbo.Tickets', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tickets
    (
        Id             INT IDENTITY(1,1) PRIMARY KEY,
        UserId         INT NOT NULL,
        CategoryId     INT NOT NULL,
        Description    NVARCHAR(1000) NOT NULL,
        PriorityId     INT NOT NULL,
        StatusId       INT NOT NULL,
        AssignedUserId INT NULL,
        CreatedAt      DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        UpdatedAt      DATETIME2 NULL,

        CONSTRAINT FK_Tickets_Users
            FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_Tickets_Categories
            FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id),
        CONSTRAINT FK_Tickets_Priorities
            FOREIGN KEY (PriorityId) REFERENCES dbo.Priorities(Id),
        CONSTRAINT FK_Tickets_Statuses
            FOREIGN KEY (StatusId) REFERENCES dbo.Statuses(Id),
        CONSTRAINT FK_Tickets_AssignedUser
            FOREIGN KEY (AssignedUserId) REFERENCES dbo.Users(Id)
    );
END;
GO

/* ---------- Заполнение справочников ---------- */

IF NOT EXISTS (SELECT 1 FROM dbo.Roles)
BEGIN
    INSERT INTO dbo.Roles (Name)
    VALUES
        (N'Пользователь'),
        (N'Специалист'),
        (N'Администратор');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Categories)
BEGIN
    INSERT INTO dbo.Categories (Name)
    VALUES
        (N'Оборудование'),
        (N'Программное обеспечение'),
        (N'Сеть'),
        (N'Учётная запись'),
        (N'Периферия'),
        (N'Другое');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Priorities)
BEGIN
    INSERT INTO dbo.Priorities (Name, SortOrder)
    VALUES
        (N'Низкий', 1),
        (N'Средний', 2),
        (N'Высокий', 3);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Statuses)
BEGIN
    INSERT INTO dbo.Statuses (Name)
    VALUES
        (N'Новая'),
        (N'В работе'),
        (N'Выполнена'),
        (N'Отклонена');
END;
GO

/* ---------- Тестовые пользователи ---------- */

IF NOT EXISTS (SELECT 1 FROM dbo.Users)
BEGIN
    INSERT INTO dbo.Users
        (Login, PasswordHash, FullName, RoleId, IsActive)
    VALUES
        (N'admin',    N'TEMP_ADMIN',   N'Администратор системы',             3, 1),
        (N'support1', N'TEMP_SUPPORT', N'Специалист технической поддержки', 2, 1),
        (N'user1',    N'TEMP_USER',    N'Тестовый пользователь',             1, 1);
END;
GO

/* ---------- Тестовые заявки ---------- */

IF NOT EXISTS (SELECT 1 FROM dbo.Tickets)
BEGIN
    INSERT INTO dbo.Tickets
        (UserId, CategoryId, Description, PriorityId, StatusId, AssignedUserId)
    VALUES
        (3, 3, N'Нет доступа в Интернет',                  3, 1, NULL),
        (3, 2, N'Не запускается Microsoft Word',           2, 2, 2),
        (3, 1, N'Не включается компьютер',                 3, 2, 2),
        (3, 5, N'Не работает принтер',                     2, 1, NULL),
        (3, 4, N'Не удаётся войти в учётную запись',       3, 4, 2);
END;
GO

/* ---------- Контрольные запросы ---------- */

SELECT name AS [База данных]
FROM sys.databases
WHERE name = N'HelpDesk_Chetverikov';
GO

SELECT TABLE_NAME AS [Таблица]
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
GO

SELECT * FROM dbo.Roles;
SELECT * FROM dbo.Categories;
SELECT * FROM dbo.Priorities ORDER BY SortOrder;
SELECT * FROM dbo.Statuses;
SELECT Id, Login, FullName, RoleId, IsActive FROM dbo.Users;
SELECT * FROM dbo.Tickets;
GO

/* Список заявок с понятными названиями (JOIN) */
SELECT
    t.Id,
    u.FullName AS [Пользователь],
    c.Name     AS [Категория],
    t.Description AS [Описание],
    p.Name     AS [Приоритет],
    s.Name     AS [Статус],
    t.CreatedAt AS [Дата создания]
FROM dbo.Tickets AS t
INNER JOIN dbo.Users      AS u ON t.UserId     = u.Id
INNER JOIN dbo.Categories AS c ON t.CategoryId = c.Id
INNER JOIN dbo.Priorities AS p ON t.PriorityId = p.Id
INNER JOIN dbo.Statuses   AS s ON t.StatusId   = s.Id
ORDER BY t.Id;
GO
