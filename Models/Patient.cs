using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalTriageSystem.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        [Display(Name = "Nom complet")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'âge est requis")]
        [Range(0, 120, ErrorMessage = "L'âge doit être entre 0 et 120")]
        [Display(Name = "Âge")]
        public int Age { get; set; }

        [EmailAddress(ErrorMessage = "Adresse email invalide")]
        [Display(Name = "Adresse email")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Numéro de téléphone invalide")]
        [Display(Name = "Téléphone")]
        public string? Phone { get; set; }

        [Display(Name = "Genre")]
        public string? Gender { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public List<Symptom>? Symptoms { get; set; }
        public TriageResult? TriageResult { get; set; }
    }
}