using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using MedicalTriageSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedicalTriageSystem.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientDashboardRealController : Controller  // Nom unique
    {
        private readonly ApplicationDbContext _context;

        public PatientDashboardRealController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()  // Utiliser Index au lieu de Dashboard
        {
            try
            {
                // Récupérer l'ID de l'utilisateur connecté
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                // Récupérer le patient depuis la base de données
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (patient == null)
                {
                    TempData["ErrorMessage"] = "Profil patient non trouvé";
                    return RedirectToAction("Login", "Auth");
                }

                // Récupérer les données réelles
                var lastTriage = await _context.TriageResults
                    .Where(tr => tr.PatientId == patient.Id)
                    .OrderByDescending(tr => tr.CreatedAt)
                    .FirstOrDefaultAsync();

                var upcomingAppointments = await _context.Appointments
                    .Include(a => a.Doctor)
                    .Where(a => a.PatientId == patient.Id && a.Date >= DateTime.Today && a.Status != "Completed")
                    .OrderBy(a => a.Date)
                    .ThenBy(a => a.StartTime)
                    .Take(5)
                    .ToListAsync();

                var recentSymptoms = await _context.Symptoms
                    .Where(s => s.PatientId == patient.Id)
                    .OrderByDescending(s => s.Date)
                    .Take(6)
                    .ToListAsync();

                var availableDoctors = await _context.Doctors
                    .Where(d => d.IsAvailable)
                    .Take(3)
                    .ToListAsync();

                // Calculer les statistiques réelles
                var patientStats = new PatientStatistics
                {
                    TotalTriages = await _context.TriageResults
                        .CountAsync(tr => tr.PatientId == patient.Id),

                    TotalSymptoms = await _context.Symptoms
                        .CountAsync(s => s.PatientId == patient.Id),

                    UpcomingAppointmentsCount = await _context.Appointments
                        .CountAsync(a => a.PatientId == patient.Id && a.Date >= DateTime.Today && a.Status != "Completed"),

                    MedicalRecordsCount = await _context.MedicalRecords
                        .CountAsync(mr => mr.PatientId == patient.Id),

                    PrescriptionsCount = await _context.Appointments
                        .CountAsync(a => a.PatientId == patient.Id && !string.IsNullOrEmpty(a.Prescription)),

                    UrgentCasesCount = await _context.TriageResults
                        .CountAsync(tr => tr.PatientId == patient.Id && tr.Level == "Urgent")
                };

                // Récupérer les dernières métriques de santé avec conversion explicite (Casting)
                var healthMetrics = new HealthMetrics
                {
                    HeartRate = (int)await GetLatestHealthMetricValue(patient.Id, "HeartRate", 72),
                    BloodPressure = (int)await GetLatestHealthMetricValue(patient.Id, "BloodPressure", 120),
                    Temperature = (double)await GetLatestHealthMetricValue(patient.Id, "Temperature", 36.6m),
                    OxygenSaturation = (int)await GetLatestHealthMetricValue(patient.Id, "OxygenSaturation", 98)
                };

                // Convertir les entités en ViewModels
                var model = new PatientDashboardSimpleViewModel
                {
                    PatientInfo = new PatientInfo
                    {
                        Id = patient.Id,
                        Name = patient.Name,
                        Age = patient.Age,
                        Gender = patient.Gender,
                        BloodType = patient.BloodType ?? "Non spécifié",
                        Allergies = patient.Allergies ?? "Aucune connue",
                        Email = patient.Email,
                        Phone = patient.Phone ?? "Non spécifié",
                        CreatedAt = patient.CreatedAt
                    },

                    LastTriageResult = lastTriage != null ? new TriageResultInfo
                    {
                        Patient = new PatientInfo { Name = patient.Name },
                        Level = lastTriage.Level,
                        Score = lastTriage.Score,
                        Recommendation = lastTriage.Recommendation,
                        Symptoms = lastTriage.Symptoms ?? "Non spécifié",
                        CreatedAt = lastTriage.CreatedAt
                    } : null,

                    HealthMetrics = healthMetrics,

                    PatientStats = patientStats,

                    UpcomingAppointments = upcomingAppointments.Select(a => new AppointmentInfo
                    {
                        Date = a.Date,
                        StartTime = a.StartTime,
                        Reason = a.Reason ?? "Consultation",
                        Doctor = a.Doctor != null ? new DoctorInfo
                        {
                            Id = a.Doctor.Id,
                            Name = a.Doctor.Name,
                            Specialty = a.Doctor.Specialty,
                            IsAvailable = a.Doctor.IsAvailable,
                            ConsultationFee = a.Doctor.ConsultationFee
                        } : null,
                        Status = a.Status ?? "Scheduled",
                        Notes = a.Notes
                    }).ToList(),

                    RecentSymptoms = recentSymptoms.Select(s => new SymptomInfo
                    {
                        Name = s.Name,
                        Description = s.Description ?? "Symptôme enregistré",
                        Date = s.Date,
                        Location = s.Location ?? "Non spécifié",
                        Duration = s.Duration ?? "Non spécifié",
                        Severity = s.Severity
                    }).ToList(),

                    DoctorAvailability = availableDoctors.Select(d => new DoctorInfo
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Specialty = d.Specialty ?? "Généraliste",
                        IsAvailable = d.IsAvailable,
                        Qualifications = d.Qualifications,
                        Availability = d.Availability,
                        ConsultationFee = d.ConsultationFee
                    }).ToList(),

                    UrgentCasesList = await _context.TriageResults
                        .Where(tr => tr.PatientId == patient.Id && tr.Level == "Urgent")
                        .OrderByDescending(tr => tr.CreatedAt)
                        .Take(3)
                        .Select(tr => new TriageResultInfo
                        {
                            Level = tr.Level,
                            Score = tr.Score,
                            Recommendation = tr.Recommendation,
                            Symptoms = tr.Symptoms ?? "Non spécifié",
                            CreatedAt = tr.CreatedAt
                        })
                        .ToListAsync()
                };

                return View("PatientDashboard", model);
            }
            catch (Exception ex)
            {
                // Log l'erreur
                TempData["ErrorMessage"] = "Une erreur est survenue lors du chargement du tableau de bord";
                return View("Error");
            }
        }

        private async Task<decimal> GetLatestHealthMetricValue(int patientId, string metricType, decimal defaultValue)
        {
            var latestMetric = await _context.HealthMetrics
                .Where(hm => hm.PatientId == patientId && hm.MetricType == metricType)
                .OrderByDescending(hm => hm.RecordedAt)
                .FirstOrDefaultAsync();

            return latestMetric != null ? latestMetric.Value : defaultValue;
        }

        // Nouvelle action pour ajouter un symptôme
        [HttpPost]
        public async Task<IActionResult> AddSymptom(string name, string description, int severity)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return Json(new { success = false, message = "Patient non trouvé" });

            var symptom = new Symptom
            {
                PatientId = patient.Id,
                Name = name,
                Description = description,
                Severity = severity,
                Date = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.Symptoms.Add(symptom);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Symptôme ajouté avec succès" });
        }

        // Nouvelle action pour prendre un rendez-vous
        [HttpPost]
        public async Task<IActionResult> BookAppointment(int doctorId, DateTime date, string reason)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return Json(new { success = false, message = "Patient non trouvé" });

            var appointment = new Appointment
            {
                PatientId = patient.Id,
                DoctorId = doctorId,
                Date = date,
                Reason = reason,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Rendez-vous demandé avec succès" });
        }

        // Nouvelle action pour mettre à jour les métriques de santé
        [HttpPost]
        public async Task<IActionResult> UpdateHealthMetrics(int heartRate, int bloodPressure, double temperature, int oxygenSaturation)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return Json(new { success = false, message = "Patient non trouvé" });

            // Créer les nouvelles métriques
            var metrics = new List<HealthMetric>
            {
                new HealthMetric
                {
                    PatientId = patient.Id,
                    MetricType = "HeartRate",
                    Value = heartRate,
                    RecordedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                },
                new HealthMetric
                {
                    PatientId = patient.Id,
                    MetricType = "BloodPressure",
                    Value = bloodPressure,
                    RecordedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                },
                new HealthMetric
                {
                    PatientId = patient.Id,
                    MetricType = "Temperature",
                    Value = (decimal)temperature,
                    RecordedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                },
                new HealthMetric
                {
                    PatientId = patient.Id,
                    MetricType = "OxygenSaturation",
                    Value = oxygenSaturation,
                    RecordedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.HealthMetrics.AddRange(metrics);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Métriques mises à jour",
                metrics = new { heartRate, bloodPressure, temperature, oxygenSaturation }
            });
        }
    }
}