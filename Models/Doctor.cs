using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalTriageSystem.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(100)]
        [Display(Name = "Nom Complet")]
        // Suppression du initialiseur string.Empty pour les champs qui peuvent être NULL en DB
        public string? Name { get; set; }

        [Required(ErrorMessage = "La spécialité est obligatoire")]
        [StringLength(100)]
        [Display(Name = "Spécialité")]
        public string? Specialty { get; set; }

        [Required(ErrorMessage = "Le numéro de licence est obligatoire")]
        [StringLength(50)]
        [Display(Name = "Numéro de Licence")]
        public string? LicenseNumber { get; set; }

        [Phone]
        [Display(Name = "Téléphone")]
        public string? Phone { get; set; }

        [EmailAddress]
        [Display(Name = "Email Professionnel")]
        public string? Email { get; set; }

        [Display(Name = "Disponible")]
        public bool IsAvailable { get; set; } = true;

        // --- CORRECTION ICI ---
        [Display(Name = "Horaires de disponibilité")]
        public string? Availability { get; set; }

        [Range(0, 60)]
        [Display(Name = "Années d'expérience")]
        public int? YearsOfExperience { get; set; }

        [Display(Name = "Qualifications / Diplômes")]
        public string? Qualifications { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Biography { get; set; }

        [Display(Name = "Frais de consultation")]
        public string? ConsultationFee { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // --- Propriétés de Navigation ---

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual List<TriageResult> TriageResults { get; set; } = new();

        public virtual List<Appointment> Appointments { get; set; } = new();
    }
}