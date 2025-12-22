using System.ComponentModel.DataAnnotations;

namespace MedicalTriageSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        [Display(Name = "Date du rendez-vous")]
        public DateTime ScheduledDate { get; set; }

        [Display(Name = "Heure de début")]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "Heure de fin")]
        public TimeSpan EndTime { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        public string? Reason { get; set; }

        public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}