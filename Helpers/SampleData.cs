using System;
using System.Collections.Generic;
using HelpDesk.Models;

namespace HelpDesk.Helpers
{
    /// <summary>
    /// Тестовые данные для проверки интерфейса до подключения MS SQL Server.
    /// </summary>
    public static class SampleData
    {
        public static List<Ticket> CreateTickets()
        {
            return new List<Ticket>
            {
                new Ticket
                {
                    Id = 1, Number = "HD-2026-000101", Author = "admin",
                    Category = "Сеть", Description = "Нет доступа к интернету в кабинете 305",
                    Priority = "Высокий", Status = "Новая", CreatedAt = new DateTime(2026, 10, 28),
                    Responsible = ""
                },
                new Ticket
                {
                    Id = 2, Number = "HD-2026-000102", Author = "admin",
                    Category = "Программное обеспечение", Description = "Ошибка при запуске Microsoft Word",
                    Priority = "Средний", Status = "В работе", CreatedAt = new DateTime(2026, 10, 28),
                    Responsible = "Петров П. П."
                },
                new Ticket
                {
                    Id = 3, Number = "HD-2026-000103", Author = "admin",
                    Category = "Оборудование", Description = "Не включается монитор на рабочем месте",
                    Priority = "Высокий", Status = "Новая", CreatedAt = new DateTime(2026, 10, 29),
                    Responsible = ""
                },
                new Ticket
                {
                    Id = 4, Number = "HD-2026-000104", Author = "admin",
                    Category = "Периферия", Description = "Не работает принтер HP LaserJet в бухгалтерии",
                    Priority = "Средний", Status = "Выполнена", CreatedAt = new DateTime(2026, 10, 27),
                    Responsible = "Сидоров С. С."
                },
                new Ticket
                {
                    Id = 5, Number = "HD-2026-000105", Author = "admin",
                    Category = "Учётная запись", Description = "Забыт пароль от корпоративной почты",
                    Priority = "Низкий", Status = "Выполнена", CreatedAt = new DateTime(2026, 10, 26),
                    Responsible = "Петров П. П."
                },
                new Ticket
                {
                    Id = 6, Number = "HD-2026-000106", Author = "admin",
                    Category = "Сеть", Description = "Медленная работа сети в кабинете 210",
                    Priority = "Средний", Status = "В работе", CreatedAt = new DateTime(2026, 10, 29),
                    Responsible = "Сидоров С. С."
                },
                new Ticket
                {
                    Id = 7, Number = "HD-2026-000107", Author = "admin",
                    Category = "Другое", Description = "Требуется установка офисного пакета",
                    Priority = "Низкий", Status = "Отклонена", CreatedAt = new DateTime(2026, 10, 25),
                    Responsible = "Петров П. П."
                },
                new Ticket
                {
                    Id = 8, Number = "HD-2026-000108", Author = "admin",
                    Category = "Периферия", Description = "Принтер печатает с полосами",
                    Priority = "Высокий", Status = "В работе", CreatedAt = new DateTime(2026, 10, 29),
                    Responsible = "Сидоров С. С."
                }
            };
        }
    }
}
