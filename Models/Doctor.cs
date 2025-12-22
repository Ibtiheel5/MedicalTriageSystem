using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalTriageSystem.Models
{
    [Table("Doctors")]
    public class Doctor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialty { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string LicenseNumber { get; set; }

        public bool IsAvailable { get; set; } = true;

        [StringLength(100)]
        public string Availability { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<TriageResult> TriageResults { get; set; } = new List<TriageResult>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}