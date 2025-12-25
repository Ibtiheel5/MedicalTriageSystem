using MedicalTriageSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalTriageSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Users.AnyAsync()) return;

            // Admin
            var admin = new User { Username = "admin", Email = "admin@medical.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "Admin", IsActive = true };
            context.Users.Add(admin);

            // Docteur
            var docUser = new User { Username = "drdupont", Email = "drdupont@medical.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("doc123"), Role = "Doctor", IsActive = true };
            context.Users.Add(docUser);

            // Patient
            var patUser = new User { Username = "patient1", Email = "patient1@gmail.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pat123"), Role = "Patient", IsActive = true };
            context.Users.Add(patUser);

            await context.SaveChangesAsync();

            var doctor = new Doctor { UserId = docUser.Id, Name = "Dr. Dupont", Specialty = "Généraliste", Email = "drdupont@medical.com", Phone = "0123456789", IsAvailable = true };
            context.Doctors.Add(doctor);

            var patient = new Patient { UserId = patUser.Id, Name = "Jean Martin", Email = "patient1@gmail.com", Phone = "0987654321", Age = 42, Gender = "Masculin" };
            context.Patients.Add(patient);

            await context.SaveChangesAsync();

            // Exemple de triage
            context.TriageResults.Add(new TriageResult
            {
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                Score = 28,
                Level = "Élevé",
                Recommendation = "Consultez rapidement un médecin.",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });

            await context.SaveChangesAsync();
        }
    }
}