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
        [ForeignKey("Patient")]
        public int PatientId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string FilePath { get; set; }

        public string FileType { get; set; }

        public long FileSize { get; set; }

        [Required]
        public DateTime RecordDate { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public virtual Patient Patient { get; set; }
    }
}