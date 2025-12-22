namespace MedicalTriageSystem.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsAvailable { get; set; }
        public string? Availability { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}