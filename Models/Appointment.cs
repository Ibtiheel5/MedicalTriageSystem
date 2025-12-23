// Models/Appointment.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalTriageSystem.Models
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public int? DoctorId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Column(TypeName = "time without time zone")]
        public TimeSpan? StartTime { get; set; } // ✅ StartTime avec 't'

        [Column(TypeName = "time without time zone")]
        public TimeSpan? EndTime { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Reason { get; set; } = string.Empty;

        [Column(TypeName = "text")]
        public string Status { get; set; } = "Scheduled";

        [Column(TypeName = "text")]
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual Doctor? Doctor { get; set; }
    }
}