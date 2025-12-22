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

        // Existing DbSets
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<TriageResult> TriageResults { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        // ✅ AJOUTER: Notifications DbSet (après avoir créé la classe Notification)
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration Patient-Symptom
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Symptoms)
                .WithOne(s => s.Patient)
                .HasForeignKey(s => s.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration Patient-TriageResult (Many-to-One)
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.TriageResults)
                .WithOne(tr => tr.Patient)
                .HasForeignKey(tr => tr.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration TriageResult-Doctor
            modelBuilder.Entity<TriageResult>()
                .HasOne(tr => tr.Doctor)
                .WithMany()
                .HasForeignKey(tr => tr.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuration Appointment
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Configuration Notification-User
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data Doctors
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

            // Seed data Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@triagemed.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Id = 2,
                    Username = "docteur1",
                    Email = "docteur@clinique.fr",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("docteur123"),
                    Role = "Doctor",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Id = 3,
                    Username = "patient1",
                    Email = "patient@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("patient123"),
                    Role = "Patient",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                }
            );

            // Seed data Patients
            modelBuilder.Entity<Patient>().HasData(
                new Patient
                {
                    Id = 1,
                    UserId = 3,
                    Name = "Jean Dupont",
                    Age = 35,
                    Email = "patient@example.com",
                    Phone = "06 12 34 56 78",
                    Gender = "Homme",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            );
        }
    }
}