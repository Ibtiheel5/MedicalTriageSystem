using System.ComponentModel.DataAnnotations;

namespace MedicalTriageSystem.Models
{
    public class Symptom
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Severity { get; set; } = "Mild"; // Mild, Moderate, Severe

        public string? Category { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}