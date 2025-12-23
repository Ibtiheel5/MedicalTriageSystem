using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalTriageSystem.Models
{
    [Table("Notifications")]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty; // ✅ Initialisé

        [Required]
        public string Message { get; set; } = string.Empty; // ✅ Initialisé

        [StringLength(50)]
        public string Type { get; set; } = "Info"; // ✅ Valeur par défaut

        [ForeignKey("User")]
        public int? UserId { get; set; }

        [ForeignKey("Patient")]
        public int? PatientId { get; set; }

        [ForeignKey("Doctor")]
        public int? DoctorId { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // ✅ CORRECTION: UtcNow

        public DateTime? ReadAt { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual Patient? Patient { get; set; }
        public virtual Doctor? Doctor { get; set; }
    }
}