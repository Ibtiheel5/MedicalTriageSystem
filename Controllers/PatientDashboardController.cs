using MedicalTriageSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalTriageSystem.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientDashboardController : Controller
    {
        public IActionResult Dashboard()
        {
            var model = new PatientDashboardSimpleViewModel
            {
                PatientInfo = new PatientInfo
                {
                    Id = 123,
                    Name = User?.Identity?.Name ?? "Patient",
                    Age = 35,
                    Gender = "Masculin",
                    BloodType = "O+",
                    Allergies = "Pollen, Pénicilline",
                    Email = "patient@email.com",
                    Phone = "06 12 34 56 78",
                    CreatedAt = DateTime.Now
                },
                LastTriageResult = new TriageResultInfo
                {
                    Level = "Moyen",
                    Score = 45,
                    Recommendation = "Consulter dans les 48h",
                    Symptoms = "Fièvre, toux",
                    CreatedAt = DateTime.Now.AddDays(-2)
                },
                HealthMetrics = new HealthMetrics
                {
                    HeartRate = 72,
                    BloodPressure = 120,
                    Temperature = 36.8,
                    OxygenSaturation = 98
                },
                PatientStats = new PatientStatistics
                {
                    TotalTriages = 5,
                    UpcomingAppointmentsCount = 2,
                    MedicalRecordsCount = 3,
                    PrescriptionsCount = 4,
                    TotalSymptoms = 12,
                    UrgentCasesCount = 1
                },
                UpcomingAppointments = new List<AppointmentInfo>
                {
                    new() {
                        Date = DateTime.Now.AddDays(3),
                        StartTime = TimeSpan.FromHours(14),
                        Reason = "Consultation de suivi",
                        Doctor = new DoctorInfo {
                            Name = "Dr. Bernard",
                            Specialty = "Généraliste"
                        },
                        Status = "Confirmed",
                        Notes = "Apporter les derniers examens"
                    }
                },
                RecentSymptoms = new List<SymptomInfo>
                {
                    new() {
                        Name = "Maux de tête",
                        Description = "Douleur frontale modérée",
                        Date = DateTime.Now.AddDays(-1),
                        Location = "Front",
                        Duration = "2 heures",
                        Severity = 5
                    }
                },
                DoctorAvailability = new List<DoctorInfo>
                {
                    new() {
                        Id = 1,
                        Name = "Dr. Bernard",
                        Specialty = "Généraliste",
                        IsAvailable = true,
                        Qualifications = "Médecin généraliste diplômé",
                        Availability = "Lun-Ven: 9h-18h",
                        ConsultationFee = "50€"
                    }
                },
                UrgentCasesList = new List<TriageResultInfo>()
            };

            return View("PatientDashboard", model);
        }
    }
}