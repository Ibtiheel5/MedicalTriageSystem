using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalTriageSystem.Models
{
    [Table("TriageResults")]
    // Models/TriageResult.cs - VERSION CORRECTE
    public class TriageResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public int? DoctorId { get; set; }

        [Required]
        public int Score { get; set; }

        [Required]
        [StringLength(20)]
        public string Level { get; set; } = "Normal";

        [Required]
        public string Recommendation { get; set; } = string.Empty;

        // ⬇️⬇️⬇️ IMPORTANT: Utilisez CreatedAt, PAS Date ⬇️⬇️⬇️
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ⬇️⬇️⬇️ SUPPRIMEZ cette ligne si elle existe ⬇️⬇️⬇️
        // public DateTime Date { get; set; }

        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual Doctor? Doctor { get; set; }
    }
}