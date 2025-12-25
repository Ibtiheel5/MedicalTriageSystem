using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedicalTriageSystem.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        public int Age { get; set; }

        [EmailAddress]
        public string? Email { get; set; } = string.Empty;

        public string? Phone { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? BloodType { get; set; } = string.Empty;
        public string? Allergies { get; set; } = string.Empty;
        public string? MedicalHistory { get; set; } = string.Empty;
        public ICollection<HealthMetric> HealthMetrics { get; set; } = new List<HealthMetric>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public List<TriageResult> TriageResults { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();
        public List<Symptom> Symptoms { get; set; } = new();
        public List<MedicalRecord> MedicalRecords { get; set; } = new();
    }
}