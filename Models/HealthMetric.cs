namespace MedicalTriageSystem.Models
{
    public class HealthMetric
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string MetricType { get; set; } = string.Empty;

        // Utilisez cette propriété pour tous les types
        public decimal Value { get; set; }

        public DateTime RecordedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public Patient? Patient { get; set; }
    }
}