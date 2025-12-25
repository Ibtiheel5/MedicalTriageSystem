using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalTriageSystem.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required]
        [StringLength(200)]
        public required string Title { get; set; }

        [Required]
        public required string Message { get; set; }

        public string Type { get; set; } = "info"; // info, success, warning, danger
        public bool IsRead { get; set; } = false;
        public string? RelatedEntityType { get; set; } // Patient, Appointment, Triage, etc.
        public int? RelatedEntityId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }

        // Navigation property
        public User? User { get; set; }
    }
}