using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalTriageSystem.Models
{
    public class TriageResult
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int? DoctorId { get; set; }

        [Range(0, 100)]
        public int Score { get; set; }

        [Required]
        public string Level { get; set; } = string.Empty;

        public string Recommendation { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }

        // Navigation properties
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}