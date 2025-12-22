using System.ComponentModel.DataAnnotations;

namespace MedicalTriageSystem.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Type { get; set; } = "Info"; // Info, Warning, Danger, Success

        public int? UserId { get; set; }
        public User? User { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ReadAt { get; set; }
    }
}