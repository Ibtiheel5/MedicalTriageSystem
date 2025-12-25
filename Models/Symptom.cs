using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalTriageSystem.Models
{
    public class Symptom
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Severity { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan? Time { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Triggers { get; set; } = string.Empty;
        public string ReliefFactors { get; set; } = string.Empty;
        public string AssociatedSymptoms { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public Patient Patient { get; set; }
    }
}