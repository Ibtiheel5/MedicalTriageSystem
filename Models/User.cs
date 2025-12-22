using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalTriageSystem.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Patient";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}