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
        [ForeignKey("Patient")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Nom")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [StringLength(50)]
        [Display(Name = "Catégorie")]
        public string Category { get; set; } // Ajouté: Cardiaque, Respiratoire, Digestif, etc.

        [StringLength(20)]
        [Display(Name = "Sévérité")]
        public string Severity { get; set; } = "Modéré"; // Léger, Modéré, Sévère

        [Display(Name = "Date de début")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        // Alias pour 'Date' si les vues l'utilisent
        [Display(Name = "Date")]
        [NotMapped] // Ne pas mapper à la base, c'est un alias
        public DateTime Date
        {
            get => StartDate;
            set => StartDate = value;
        }

        [Display(Name = "Date de fin")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Durée")]
        public string Duration { get; set; } // Aigu (<7j), Subaigu (7-30j), Chronique (>30j)

        [Display(Name = "Fréquence")]
        public string Frequency { get; set; } // Constant, Intermittent, Occasionnel

        [Display(Name = "Intensité")]
        [Range(1, 10)]
        public int? Intensity { get; set; } // 1-10

        [Display(Name = "Facteurs aggravants")]
        public string AggravatingFactors { get; set; }

        [Display(Name = "Facteurs soulageants")]
        public string RelievingFactors { get; set; }

        [Display(Name = "Actif")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Display(Name = "Créé le")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Mis à jour le")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation property
        [Display(Name = "Patient")]
        public virtual Patient Patient { get; set; }
    }
}