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
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Utiliser 'timestamp with time zone' pour tous les DateTime
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("timestamp with time zone");
                    }
                }
            }

            // Configuration des relations

            // User ↔ Patient (One-to-One)
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User ↔ Doctor (One-to-One)
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Patient ↔ TriageResults (One-to-Many)
            modelBuilder.Entity<TriageResult>()
                .HasOne(tr => tr.Patient)
                .WithMany(p => p.TriageResults)
                .HasForeignKey(tr => tr.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Doctor ↔ TriageResults (One-to-Many)
            modelBuilder.Entity<TriageResult>()
                .HasOne(tr => tr.Doctor)
                .WithMany(d => d.TriageResults)
                .HasForeignKey(tr => tr.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Patient ↔ Symptoms (One-to-Many)
            modelBuilder.Entity<Symptom>()
                .HasOne(s => s.Patient)
                .WithMany(p => p.Symptoms)
                .HasForeignKey(s => s.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Patient ↔ Appointments (One-to-Many)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Doctor ↔ Appointments (One-to-Many)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Patient ↔ MedicalRecords (One-to-Many)
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(mr => mr.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification relations
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

            // TriageResult - SANS Date, seulement CreatedAt
            modelBuilder.Entity<TriageResult>()
                .Property(tr => tr.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Appointment - Configuration selon la structure de la table
            modelBuilder.Entity<Appointment>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Appointment>()
                .Property(a => a.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // ✅ CORRECTION : Configuration des colonnes spécifiques d'Appointment
            modelBuilder.Entity<Appointment>()
                .Property(a => a.Date)
                .HasColumnType("date");

            modelBuilder.Entity<Appointment>()
                .Property(a => a.StartTime)
                .HasColumnType("time without time zone");

            modelBuilder.Entity<Appointment>()
                .Property(a => a.EndTime)
                .HasColumnType("time without time zone");

            modelBuilder.Entity<Appointment>()
                .Property(a => a.Reason)
                .HasColumnType("text");

            modelBuilder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasColumnType("text")
                .HasDefaultValue("Scheduled");

            modelBuilder.Entity<Appointment>()
                .Property(a => a.Notes)
                .HasColumnType("text");

            modelBuilder.Entity<Notification>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<MedicalRecord>()
                .Property(mr => mr.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Configuration pour Symptom
            // Dans OnModelCreating, section Symptom :
            modelBuilder.Entity<Symptom>()
                .Property(s => s.Name)
                .HasColumnType("text");

            modelBuilder.Entity<Symptom>()
                .Property(s => s.Description)
                .HasColumnType("text");

            modelBuilder.Entity<Symptom>()
                .Property(s => s.Severity)
                .HasColumnType("integer");

            modelBuilder.Entity<Symptom>()
                .Property(s => s.Category)
                .HasColumnType("text");

            modelBuilder.Entity<Symptom>()
                .Property(s => s.Date)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // ⚠️ SUPPRIMEZ toutes les autres configurations pour Symptom
            // qui font référence à des colonnes qui n'existent pas
        }
        public override int SaveChanges()
        {
            UpdateDateTimeKinds();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateDateTimeKinds();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateDateTimeKinds()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BaseEntity baseEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            baseEntity.CreatedAt = DateTime.UtcNow;
                            baseEntity.UpdatedAt = DateTime.UtcNow;
                            break;

                        case EntityState.Modified:
                            baseEntity.UpdatedAt = DateTime.UtcNow;
                            // Ne pas toucher CreatedAt
                            entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                            break;
                    }
                }
            }
        }
    }

}