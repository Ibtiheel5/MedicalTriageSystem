using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalTriageSystem.Models
{
    [Table("MedicalRecords")]
    public class MedicalRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [StringLength(100)]
        public string RecordType { get; set; } = string.Empty; // Consultation, Test, Hospitalisation

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty; // Pour stocker des fichiers PDF/images

        public string FileType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime RecordDate { get; set; } = DateTime.UtcNow;

        public string DoctorName { get; set; } = string.Empty;

        public string HospitalClinic { get; set; } = string.Empty;

        public string TestResults { get; set; } = string.Empty;

        public string Medications { get; set; } = string.Empty;

        public string Procedures { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public bool IsConfidential { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
    }
}