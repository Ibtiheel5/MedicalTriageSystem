namespace MedicalTriageSystem.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public string Availability { get; set; } = "9h-17h";
    }
}