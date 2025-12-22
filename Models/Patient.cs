using System.ComponentModel.DataAnnotations;

namespace MedicalTriageSystem.Models
{
    public class Patient
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // ✅ SEULEMENT TriageResults (pluriel) - PAS de TriageResult (singulier)
        public virtual List<Symptom> Symptoms { get; set; } = new();
        public virtual List<TriageResult> TriageResults { get; set; } = new();
        public virtual List<Appointment> Appointments { get; set; } = new();
    }
}