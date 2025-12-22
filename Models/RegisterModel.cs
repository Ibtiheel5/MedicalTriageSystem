// Models/RegisterModel.cs
using System.ComponentModel.DataAnnotations;

namespace MedicalTriageSystem.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        [Display(Name = "Nom d'utilisateur")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Le nom d'utilisateur doit contenir entre 3 et 50 caractères")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Adresse email invalide")]
        [Display(Name = "Adresse email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmation du mot de passe est requise")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le rôle est requis")]
        [Display(Name = "Rôle")]
        public string Role { get; set; } = "Patient";

        [Display(Name = "Nom complet")]
        public string? FullName { get; set; }

        [Display(Name = "Numéro de téléphone")]
        [Phone(ErrorMessage = "Numéro de téléphone invalide")]
        public string? Phone { get; set; }
    }
}