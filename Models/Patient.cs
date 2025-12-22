using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalTriageSystem.Models
{
    [Table("Patients")]
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public int Age { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(20)]
        public string Gender { get; set; } = "Non spécifié";

        [StringLength(10)]
        public string BloodType { get; set; } = "Non spécifié";

        public string Address { get; set; }

        public string EmergencyContact { get; set; }

        public string EmergencyPhone { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual User User { get; set; }

        // Collections (initialisez-les comme des listes vides)
        public virtual ICollection<TriageResult> TriageResults { get; set; } = new List<TriageResult>();
        public virtual ICollection<Symptom> Symptoms { get; set; } = new List<Symptom>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}