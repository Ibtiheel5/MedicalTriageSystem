using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalTriageSystem.Models
{
    [Table("TriageResults")]
    public class TriageResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Patient")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [ForeignKey("Doctor")]
        [Display(Name = "Médecin")]
        public int? DoctorId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Niveau")]
        public string Level { get; set; } = "Normal"; // Urgent, Élevé, Normal, Faible

        [Range(0, 100)]
        [Display(Name = "Score")]
        public int Score { get; set; }

        [Required]
        [Display(Name = "Symptômes")]
        public string Symptoms { get; set; }

        [Required]
        [Display(Name = "Recommandation")]
        public string Recommendation { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Required]
        [Display(Name = "Date du triage")]
        public DateTime Date { get; set; } = DateTime.Now;

        // Alias pour CreatedAt si nécessaire
        [Display(Name = "Créé le")]
        public DateTime CreatedAt
        {
            get => Date;
            set => Date = value;
        }

        // Navigation properties
        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}