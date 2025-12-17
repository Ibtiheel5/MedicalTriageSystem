using MedicalTriageSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalTriageSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<TriageResult> TriageResults { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration Patient - Symptoms (One-to-Many)
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Symptoms)
                .WithOne(s => s.Patient)
                .HasForeignKey(s => s.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration Patient - TriageResult (One-to-One)
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.TriageResult)
                .WithOne(tr => tr.Patient)
                .HasForeignKey<TriageResult>(tr => tr.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration TriageResult - Doctor (Many-to-One)
            modelBuilder.Entity<TriageResult>()
                .HasOne(tr => tr.Doctor)
                .WithMany()
                .HasForeignKey(tr => tr.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Données de départ pour les médecins
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor
                {
                    Id = 1,
                    Name = "Dr. Martin Dubois",
                    Specialty = "Médecine Générale",
                    Phone = "01 23 45 67 89",
                    Email = "m.dubois@clinique.fr",
                    IsAvailable = true,
                    Availability = "Lun-Ven: 9h-18h"
                },
                new Doctor
                {
                    Id = 2,
                    Name = "Dr. Sophie Laurent",
                    Specialty = "Urgentiste",
                    Phone = "01 98 76 54 32",
                    Email = "s.laurent@clinique.fr",
                    IsAvailable = true,
                    Availability = "24/7 - Urgences"
                },
                new Doctor
                {
                    Id = 3,
                    Name = "Dr. Jean Moreau",
                    Specialty = "Cardiologie",
                    Phone = "01 12 34 56 78",
                    Email = "j.moreau@clinique.fr",
                    IsAvailable = false,
                    Availability = "Mar-Jeu: 10h-16h"
                }
            );
        }
    }
}