using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalTriageSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int? DoctorId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        [Required]
        [StringLength(200)]
        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = "Scheduled";
        public string Prescription { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string FollowUpInstructions { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}