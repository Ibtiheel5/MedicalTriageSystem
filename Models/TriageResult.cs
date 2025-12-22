using System.ComponentModel.DataAnnotations;

namespace MedicalTriageSystem.Models
{
    public class TriageResult
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        [Required]
        public string Level { get; set; } = "Normal"; // Urgent, High, Normal, Low

        public int Score { get; set; }

        [Required]
        public string Recommendation { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime Date { get; set; } = DateTime.Now;
    }
}