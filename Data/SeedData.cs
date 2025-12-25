using MedicalTriageSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalTriageSystem.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Seed Users
            if (!await context.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new User
                    {
                        Username = "admin",
                        Email = "admin@medicaltriage.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                        Role = "Admin",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "dr.dubois",
                        Email = "dr.dubois@medicaltriage.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor123!"),
                        Role = "Doctor",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "jean.dupont",
                        Email = "jean.dupont@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient123!"),
                        Role = "Patient",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "marie.curie",
                        Email = "marie.curie@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient123!"),
                        Role = "Patient",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }
            // Seed Health Metrics
            if (!await context.HealthMetrics.AnyAsync())
            {
                var patient1 = await context.Patients.FirstAsync(p => p.Name == "Jean Dupont");

                var healthMetrics = new List<HealthMetric>
    {
        new HealthMetric
        {
            PatientId = patient1.Id,
            MetricType = "HeartRate",
            Value = 72,
            RecordedAt = DateTime.UtcNow.AddHours(-2),
            CreatedAt = DateTime.UtcNow.AddHours(-2)
        },
        new HealthMetric
        {
            PatientId = patient1.Id,
            MetricType = "BloodPressure",
            Value = 120,
            RecordedAt = DateTime.UtcNow.AddHours(-1),
            CreatedAt = DateTime.UtcNow.AddHours(-1)
        },
        new HealthMetric
        {
            PatientId = patient1.Id,
            MetricType = "Temperature",
            Value = 36.6m,
            RecordedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        },
        new HealthMetric
        {
            PatientId = patient1.Id,
            MetricType = "OxygenSaturation",
            Value = 98,
            RecordedAt = DateTime.UtcNow.AddMinutes(-30),
            CreatedAt = DateTime.UtcNow.AddMinutes(-30)
        }
    };

                await context.HealthMetrics.AddRangeAsync(healthMetrics);
                await context.SaveChangesAsync();
            }

            // Seed Doctors
            if (!await context.Doctors.AnyAsync())
            {
                var doctors = new List<Doctor>
                {
                    new Doctor
                    {
                        UserId = (await context.Users.FirstAsync(u => u.Username == "dr.dubois")).Id,
                        Name = "Dr. Martin Dubois",
                        Specialty = "Médecine Générale",
                        LicenseNumber = "MED123456",
                        Phone = "01 23 45 67 89",
                        Email = "m.dubois@clinique.fr",
                        IsAvailable = true,
                        Availability = "Lun-Ven: 9h-18h | Sam: 9h-13h",
                        YearsOfExperience = 15,
                        Qualifications = "MD, Spécialiste en Médecine Générale",
                        Biography = "Spécialiste avec plus de 15 ans d'expérience en médecine générale.",
                        ConsultationFee = "60€",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Doctor
                    {
                        Name = "Dr. Sophie Laurent",
                        Specialty = "Cardiologie",
                        LicenseNumber = "CARD789012",
                        Phone = "01 98 76 54 32",
                        Email = "s.laurent@cardiologie.fr",
                        IsAvailable = true,
                        Availability = "Lun-Mer-Ven: 8h-16h",
                        YearsOfExperience = 12,
                        Qualifications = "MD, Cardiologie, Échographie cardiaque",
                        Biography = "Cardiologue spécialisée dans les maladies cardiaques.",
                        ConsultationFee = "85€",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Doctor
                    {
                        Name = "Dr. Pierre Moreau",
                        Specialty = "Pédiatrie",
                        LicenseNumber = "PED456789",
                        Phone = "01 45 67 89 01",
                        Email = "p.moreau@pediatrie.fr",
                        IsAvailable = true,
                        Availability = "Mar-Jeu-Sam: 10h-18h",
                        YearsOfExperience = 8,
                        Qualifications = "MD, Pédiatrie, Néonatologie",
                        Biography = "Pédiatre spécialisé dans les soins aux enfants.",
                        ConsultationFee = "55€",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                await context.Doctors.AddRangeAsync(doctors);
                await context.SaveChangesAsync();
            }

            // Seed Patients
            if (!await context.Patients.AnyAsync())
            {
                var patients = new List<Patient>
                {
                    new Patient
                    {
                        UserId = (await context.Users.FirstAsync(u => u.Username == "jean.dupont")).Id,
                        Name = "Jean Dupont",
                        Age = 42,
                        Email = "jean.dupont@example.com",
                        Phone = "06 12 34 56 78",
                        Gender = "Homme",
                        Address = "123 Rue de Paris, 75001 Paris",
                        BloodType = "O+",
                        Allergies = "Pénicilline, Pollen",
                        MedicalHistory = "Hypertension légère, Asthme",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Patient
                    {
                        UserId = (await context.Users.FirstAsync(u => u.Username == "marie.curie")).Id,
                        Name = "Marie Curie",
                        Age = 35,
                        Email = "marie.curie@example.com",
                        Phone = "06 98 76 54 32",
                        Gender = "Femme",
                        Address = "456 Avenue des Champs, 75008 Paris",
                        BloodType = "A+",
                        Allergies = "Aucune",
                        MedicalHistory = "Suivi gynécologique régulier",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                await context.Patients.AddRangeAsync(patients);
                await context.SaveChangesAsync();
            }

            // Seed Triage Results
            if (!await context.TriageResults.AnyAsync())
            {
                var patient1 = await context.Patients.FirstAsync(p => p.Name == "Jean Dupont");
                var patient2 = await context.Patients.FirstAsync(p => p.Name == "Marie Curie");
                var doctor1 = await context.Doctors.FirstAsync(d => d.Name == "Dr. Martin Dubois");

                var triageResults = new List<TriageResult>
                {
                    new TriageResult
                    {
                        PatientId = patient1.Id,
                        DoctorId = doctor1.Id,
                        Score = 75,
                        Level = "Élevé",
                        Recommendation = "Consulter un médecin dans les 24h. Symptômes nécessitant une évaluation médicale rapide.",
                        Symptoms = "Douleur thoracique, essoufflement, fatigue",
                        Notes = "Patient avec antécédents d'hypertension",
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        ReviewedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new TriageResult
                    {
                        PatientId = patient1.Id,
                        Score = 45,
                        Level = "Moyen",
                        Recommendation = "Prendre rendez-vous cette semaine. Surveiller les symptômes.",
                        Symptoms = "Toux persistante, légère fièvre",
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new TriageResult
                    {
                        PatientId = patient2.Id,
                        Score = 85,
                        Level = "Urgent",
                        Recommendation = "Consulter immédiatement aux urgences. Symptômes potentiellement graves.",
                        Symptoms = "Fortes douleurs abdominales, nausées, vertiges",
                        Notes = "À surveiller de près",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new TriageResult
                    {
                        PatientId = patient2.Id,
                        Score = 25,
                        Level = "Faible",
                        Recommendation = "Surveillance à domicile. Repos et hydratation recommandés.",
                        Symptoms = "Maux de tête légers, fatigue",
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    }
                };

                await context.TriageResults.AddRangeAsync(triageResults);
                await context.SaveChangesAsync();
            }

            // Seed Appointments
            if (!await context.Appointments.AnyAsync())
            {
                var patient1 = await context.Patients.FirstAsync(p => p.Name == "Jean Dupont");
                var patient2 = await context.Patients.FirstAsync(p => p.Name == "Marie Curie");
                var doctor1 = await context.Doctors.FirstAsync(d => d.Name == "Dr. Martin Dubois");
                var doctor2 = await context.Doctors.FirstAsync(d => d.Name == "Dr. Sophie Laurent");

                var appointments = new List<Appointment>
                {
                    new Appointment
                    {
                        PatientId = patient1.Id,
                        DoctorId = doctor1.Id,
                        Date = DateTime.UtcNow.AddDays(2).Date,
                        StartTime = new TimeSpan(14, 30, 0),
                        EndTime = new TimeSpan(15, 0, 0),
                        Reason = "Suivi hypertension",
                        Status = "Confirmed",
                        Notes = "Vérifier tension artérielle",
                        CreatedAt = DateTime.UtcNow.AddDays(-3),
                        UpdatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new Appointment
                    {
                        PatientId = patient1.Id,
                        DoctorId = doctor2.Id,
                        Date = DateTime.UtcNow.AddDays(5).Date,
                        StartTime = new TimeSpan(10, 0, 0),
                        EndTime = new TimeSpan(10, 30, 0),
                        Reason = "Examen cardiaque",
                        Status = "Scheduled",
                        Notes = "Échographie cardiaque programmée",
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        UpdatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new Appointment
                    {
                        PatientId = patient2.Id,
                        DoctorId = doctor1.Id,
                        Date = DateTime.UtcNow.AddDays(1).Date,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(9, 30, 0),
                        Reason = "Consultation générale",
                        Status = "Confirmed",
                        Notes = "Bilan de santé annuel",
                        CreatedAt = DateTime.UtcNow.AddDays(-7),
                        UpdatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new Appointment
                    {
                        PatientId = patient2.Id,
                        Date = DateTime.UtcNow.AddDays(7).Date,
                        StartTime = new TimeSpan(11, 0, 0),
                        Reason = "Vaccination grippe",
                        Status = "Scheduled",
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                await context.Appointments.AddRangeAsync(appointments);
                await context.SaveChangesAsync();
            }

            // Seed Symptoms
            if (!await context.Symptoms.AnyAsync())
            {
                var patient1 = await context.Patients.FirstAsync(p => p.Name == "Jean Dupont");

                var symptoms = new List<Symptom>
                {
                    new Symptom
                    {
                        PatientId = patient1.Id,
                        Name = "Douleur thoracique",
                        Description = "Douleur au centre de la poitrine, sensation de serrement",
                        Severity = 8,
                        Date = DateTime.UtcNow.AddDays(-2).Date,
                        Time = new TimeSpan(14, 30, 0),
                        Location = "Centre de la poitrine",
                        Duration = "30 minutes",
                        Frequency = "Première fois",
                        Triggers = "Effort physique",
                        ReliefFactors = "Repos",
                        AssociatedSymptoms = "Essoufflement, transpiration",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new Symptom
                    {
                        PatientId = patient1.Id,
                        Name = "Essoufflement",
                        Description = "Difficulté à respirer profondément",
                        Severity = 6,
                        Date = DateTime.UtcNow.AddDays(-2).Date,
                        Location = "Poumons",
                        Duration = "1 heure",
                        Frequency = "Intermittent",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new Symptom
                    {
                        PatientId = patient1.Id,
                        Name = "Fatigue",
                        Description = "Fatigue générale, manque d'énergie",
                        Severity = 4,
                        Date = DateTime.UtcNow.AddDays(-3).Date,
                        Duration = "3 jours",
                        Frequency = "Constant",
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    }
                };

                await context.Symptoms.AddRangeAsync(symptoms);
                await context.SaveChangesAsync();
            }

            // Seed Medical Records
            if (!await context.MedicalRecords.AnyAsync())
            {
                var patient1 = await context.Patients.FirstAsync(p => p.Name == "Jean Dupont");

                var medicalRecords = new List<MedicalRecord>
                {
                    new MedicalRecord
                    {
                        PatientId = patient1.Id,
                        RecordType = "Test",
                        Title = "Analyse sanguine complète",
                        Description = "Bilan sanguin complet avec hémogramme, glycémie, cholestérol",
                        RecordDate = DateTime.UtcNow.AddMonths(-3).Date,
                        DoctorName = "Dr. Martin Dubois",
                        HospitalClinic = "Laboratoire Central",
                        TestResults = "Glycémie: 1.10 g/L, Cholestérol total: 2.10 g/L, Triglycérides: 1.05 g/L",
                        Notes = "Résultats dans les normes",
                        CreatedAt = DateTime.UtcNow.AddMonths(-3),
                        UpdatedAt = DateTime.UtcNow.AddMonths(-3)
                    },
                    new MedicalRecord
                    {
                        PatientId = patient1.Id,
                        RecordType = "Consultation",
                        Title = "Consultation cardiologie",
                        Description = "Consultation de suivi cardiaque",
                        RecordDate = DateTime.UtcNow.AddMonths(-6).Date,
                        DoctorName = "Dr. Sophie Laurent",
                        HospitalClinic = "Clinique du Cœur",
                        Medications = "Aspirine 100mg/jour, Atorvastatine 20mg/jour",
                        Procedures = "ECG normal, Échographie cardiaque normale",
                        Notes = "Patient stable, poursuivre traitement",
                        CreatedAt = DateTime.UtcNow.AddMonths(-6),
                        UpdatedAt = DateTime.UtcNow.AddMonths(-6)
                    }
                };

                await context.MedicalRecords.AddRangeAsync(medicalRecords);
                await context.SaveChangesAsync();
            }

            Console.WriteLine("✅ Database seeding completed successfully!");
        }
    }
}