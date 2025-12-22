using System.ComponentModel.DataAnnotations;

namespace MedicalTriageSystem.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
        [Display(Name = "Nom d'utilisateur")]
        public string Username { get; set; }

        [Required(ErrorMessage = "L'adresse email est obligatoire")]
        [EmailAddress(ErrorMessage = "Veuillez entrer une adresse email valide")]
        [Display(Name = "Adresse email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [Required(ErrorMessage = "La confirmation du mot de passe est obligatoire")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas")]
        [Display(Name = "Confirmer le mot de passe")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Nom complet")]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Veuillez entrer un numéro de téléphone valide")]
        [Display(Name = "Téléphone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Le type de compte est obligatoire")]
        [Display(Name = "Type de compte")]
        public string Role { get; set; } = "Patient";
    }
}