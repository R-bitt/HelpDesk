using System;

namespace HelpDesk.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Responsible { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
