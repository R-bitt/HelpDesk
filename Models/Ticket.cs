using System;

namespace HelpDesk.Models
{
    /// <summary>
    /// Заявка HelpDesk. Идентификаторы нужны для связи с базой данных,
    /// а строковые названия — для отображения в интерфейсе.
    /// </summary>
    public class Ticket
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int CategoryId { get; set; }
        public string Category { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int PriorityId { get; set; }
        public string Priority { get; set; } = string.Empty;

        public int StatusId { get; set; }
        public string Status { get; set; } = string.Empty;

        public int? AssignedUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
