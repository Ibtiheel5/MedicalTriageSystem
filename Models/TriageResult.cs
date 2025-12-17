namespace MedicalTriageSystem.Models
{
    public class TriageResult
    {
        public int Id { get; set; }
        public string Level { get; set; } = string.Empty; // Urgent, Normal, Faible
        public string Recommendation { get; set; } = string.Empty;
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime Date => CreatedAt; // Propriété pour compatibilité
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
    }
}