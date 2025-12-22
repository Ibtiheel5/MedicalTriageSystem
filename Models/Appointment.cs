using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalTriageSystem.Models
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Patient")]
        public int PatientId { get; set; }

        [ForeignKey("Doctor")]
        public int? DoctorId { get; set; }

        [Required]
        [Display(Name = "Date de rendez-vous")]
        public DateTime AppointmentDate { get; set; } // Renommé pour correspondre aux vues

        // Ajoutez ces propriétés si vos vues les utilisent
        [Display(Name = "Date programmée")]
        public DateTime ScheduledDate
        {
            get => AppointmentDate;
            set => AppointmentDate = value;
        }

        [Display(Name = "Heure de début")]
        public TimeSpan StartTime
        {
            get => AppointmentDate.TimeOfDay;
            set => AppointmentDate = AppointmentDate.Date + value;
        }

        [Display(Name = "Heure de fin")]
        public TimeSpan EndTime { get; set; } // Vous pouvez calculer ou stocker

        [StringLength(50)]
        [Display(Name = "Type")]
        public string Type { get; set; } = "Consultation"; // Consultation, Suivi, Urgence

        [StringLength(20)]
        [Display(Name = "Statut")]
        public string Status { get; set; } = "Scheduled"; // Programmé, Confirmé, Annulé, Terminé

        [Display(Name = "Raison")]
        public string Reason { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Display(Name = "Créé le")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [Display(Name = "Patient")]
        public virtual Patient Patient { get; set; }

        [Display(Name = "Médecin")]
        public virtual Doctor Doctor { get; set; }
    }
}