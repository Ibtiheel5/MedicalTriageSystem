using Microsoft.EntityFrameworkCore;
using MedicalTriageSystem.Models;

namespace MedicalTriageSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<TriageResult> TriageResults { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; } // Ajouté

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurez tous les DateTime pour qu'ils soient en UTC
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        // Force UTC pour PostgreSQL
                        property.SetColumnType("timestamp without time zone");

                        // Configure la valeur par défaut
                        if (property.Name.Contains("CreatedAt") ||
                            property.Name.Contains("UpdatedAt") ||
                            property.Name.Contains("Date"))
                        {
                            property.SetDefaultValueSql("CURRENT_TIMESTAMP");
                        }
                    }
                }
            }

            // Configuration des relations
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TriageResult>()
                .HasOne(tr => tr.Patient)
                .WithMany(p => p.TriageResults)
                .HasForeignKey(tr => tr.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TriageResult>()
                .HasOne(tr => tr.Doctor)
                .WithMany(d => d.TriageResults)
                .HasForeignKey(tr => tr.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Symptom>()
               .HasOne(s => s.Patient)
               .WithMany(p => p.Symptoms)
               .HasForeignKey(s => s.PatientId)
               .OnDelete(DeleteBehavior.Cascade);

            // Valeurs par défaut pour Symptom
            modelBuilder.Entity<Symptom>()
                .Property(s => s.StartDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Symptom>()
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Symptom>()
                .Property(s => s.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(mr => mr.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Patient)
                .WithMany()
                .HasForeignKey(n => n.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Doctor)
                .WithMany()
                .HasForeignKey(n => n.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Contraintes uniques
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.Email)
                .IsUnique();

            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.LicenseNumber)
                .IsUnique();

            // Valeurs par défaut
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Patient>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Patient>()
                .Property(p => p.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<TriageResult>()
                .Property(tr => tr.Date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Appointment>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Notification>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<MedicalRecord>()
                .Property(mr => mr.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Configuration pour AppointmentDate
            modelBuilder.Entity<Appointment>()
                .Property(a => a.AppointmentDate)
                .HasColumnType("timestamp without time zone");
        }
    }
}