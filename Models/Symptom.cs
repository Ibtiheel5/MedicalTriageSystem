// Models/Symptom.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalTriageSystem.Models
{
    [Table("Symptoms")]
    public class Symptom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Name { get; set; } = string.Empty; // ✅ Colonne Name (text)

        [Column(TypeName = "text")]
        public string Description { get; set; } = string.Empty; // ✅ Colonne Description (text)

        [Required]
        public int Severity { get; set; } = 3; // ✅ Colonne Severity (integer), 1-5 échelle

        [Column(TypeName = "text")]
        public string Category { get; set; } = "General"; // ✅ Colonne Category (text)

        [Required]
        [Column(TypeName = "timestamp without time zone")]
        public DateTime Date { get; set; } = DateTime.UtcNow; // ✅ Colonne Date (pas StartDate)

        // ⚠️ PAS de AggravatingFactors, RelievingFactors, StartDate, EndDate, etc.
        // Ces colonnes n'existent pas dans votre table

        // Navigation property
        public virtual Patient Patient { get; set; } = null!;
    }
}