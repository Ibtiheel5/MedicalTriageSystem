using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalTriageSystem.Models
{
    // Ajout de l'héritage : BaseEntity pour récupérer CreatedAt et UpdatedAt
    public class User : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        public required string Role { get; set; } = "Patient";

        public bool IsActive { get; set; } = true;

        // Note: CreatedAt et UpdatedAt sont maintenant hérités de BaseEntity

        // Navigation properties
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
    }
}