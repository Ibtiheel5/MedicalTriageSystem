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

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<TriageResult> TriageResults { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<HealthMetric> HealthMetrics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            });
            // HealthMetric configuration
            modelBuilder.Entity<HealthMetric>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

                entity.HasOne(hm => hm.Patient)
                    .WithMany(p => p.HealthMetrics)
                    .HasForeignKey(hm => hm.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Patient configuration
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => e.Email);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

                entity.HasOne(p => p.User)
                    .WithOne(u => u.Patient)
                    .HasForeignKey<Patient>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Doctor configuration
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.LicenseNumber).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

                entity.HasOne(d => d.User)
                    .WithOne(u => u.Doctor)
                    .HasForeignKey<Doctor>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // TriageResult configuration
            modelBuilder.Entity<TriageResult>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

                entity.HasOne(tr => tr.Patient)
                    .WithMany(p => p.TriageResults)
                    .HasForeignKey(tr => tr.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tr => tr.Doctor)
                    .WithMany(d => d.TriageResults)
                    .HasForeignKey(tr => tr.DoctorId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Appointment configuration
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

                entity.HasOne(a => a.Patient)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(a => a.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Doctor)
                    .WithMany(d => d.Appointments)
                    .HasForeignKey(a => a.DoctorId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Symptom configuration
            modelBuilder.Entity<Symptom>(entity =>
            {
                entity.Property(e => e.Date).HasDefaultValueSql("NOW()");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

                entity.HasOne(s => s.Patient)
                    .WithMany(p => p.Symptoms)
                    .HasForeignKey(s => s.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MedicalRecord configuration
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

                entity.HasOne(mr => mr.Patient)
                    .WithMany(p => p.MedicalRecords)
                    .HasForeignKey(mr => mr.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}