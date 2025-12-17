namespace MedicalTriageSystem.Models
{
    public class Symptom
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; // Ajouté
        public int Severity { get; set; } // 1-10
        public string Category { get; set; } = string.Empty; // Urgent, Normal, Faible
        public DateTime Date { get; set; } = DateTime.Now; // Ajouté
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
    }
}