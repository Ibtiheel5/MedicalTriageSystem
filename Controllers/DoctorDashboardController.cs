using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using MedicalTriageSystem.Models.ViewModels;
using MedicalTriageSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedicalTriageSystem.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public DoctorDashboardController(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            // 1. On récupère l'ID de l'utilisateur connecté
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Auth");

            int userId = int.Parse(userIdClaim);

            // 2. On cherche le docteur lié à cet ID
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            // SI C'EST ICI QUE ÇA COUPE :
            if (doctor == null)
            {
                // Message d'erreur plus précis pour vous aider
                TempData["ErrorMessage"] = $"Accès refusé : Aucun profil Docteur n'est lié à l'utilisateur ID {userId}.";
                return RedirectToAction("Index", "Home");
            }

            var today = DateTime.UtcNow.Date;

            // Get global statistics
            var stats = new DashboardStatsViewModel
            {
                TotalPatients = await _context.Patients.CountAsync(),
                TotalDoctors = await _context.Doctors.CountAsync(),
                TotalTriages = await _context.TriageResults.CountAsync(),
                UrgentCasesCount = await _context.TriageResults.CountAsync(tr => tr.Level == "Urgent"),
                TodayTriagesCount = await _context.TriageResults.CountAsync(tr => tr.CreatedAt.Date == today),
                WeekTriagesCount = await _context.TriageResults.CountAsync(tr => tr.CreatedAt >= today.AddDays(-7)),
                MonthlyTriagesCount = await _context.TriageResults.CountAsync(tr => tr.CreatedAt >= new DateTime(today.Year, today.Month, 1)),
                AvgTriageTime = TimeSpan.FromMinutes(15),
                AvgResponseTime = 8.5m,
                PatientSatisfaction = 96.2m
            };

            // Get doctor-specific statistics
            var doctorStats = new DoctorStatsViewModel
            {
                MyPatientsCount = await _context.Appointments
                    .Where(a => a.DoctorId == doctor.Id)
                    .Select(a => a.PatientId)
                    .Distinct()
                    .CountAsync(),
                MyAppointmentsToday = await _context.Appointments
                    .CountAsync(a => a.DoctorId == doctor.Id && a.Date.Date == today),
                MyAppointmentsThisWeek = await _context.Appointments
                    .CountAsync(a => a.DoctorId == doctor.Id && a.Date >= today && a.Date <= today.AddDays(7)),
                MyUrgentCasesCount = await _context.TriageResults
                    .CountAsync(tr => tr.DoctorId == doctor.Id && tr.Level == "Urgent"),
                AverageConsultationTime = 22.5m,
                PatientsSeenToday = await _context.Appointments
                    .CountAsync(a => a.DoctorId == doctor.Id && a.Date.Date == today && a.Status == "Completed"),
                CompletedAppointmentsCount = await _context.Appointments
                    .CountAsync(a => a.DoctorId == doctor.Id && a.Status == "Completed"),
                PatientRating = 4.7m
            };

            // Get today's appointments
            var todayAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctor.Id && a.Date.Date == today)
                .OrderBy(a => a.StartTime)
                .ToListAsync();

            // Get upcoming appointments
            var upcomingAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctor.Id && a.Date >= today && a.Status != "Completed")
                .OrderBy(a => a.Date).ThenBy(a => a.StartTime)
                .Take(10)
                .ToListAsync();

            // Get urgent cases
            var urgentCases = await _context.TriageResults
                .Include(tr => tr.Patient)
                .Where(tr => tr.Level == "Urgent" && tr.DoctorId == doctor.Id)
                .OrderByDescending(tr => tr.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Get my patients
            var myPatients = await GetMyPatients(doctor.Id);

            // Get available doctors
            var availableDoctors = await _context.Doctors
                .Where(d => d.IsAvailable && d.Id != doctor.Id)
                .Take(5)
                .ToListAsync();

            // Get appointment requests
            var appointmentRequests = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctor.Id && a.Status == "Pending")
                .Select(a => new AppointmentRequestViewModel
                {
                    Id = a.Id,
                    PatientName = a.Patient.Name,
                    Reason = a.Reason,
                    PreferredDate = a.Date,
                    PreferredTime = a.StartTime.HasValue ? a.StartTime.Value.ToString(@"hh\:mm") : "",
                    RequestedAt = a.CreatedAt,
                    Status = a.Status
                })
                .Take(5)
                .ToListAsync();

            // Create view model
            var viewModel = new DoctorDashboardViewModel
            {
                DoctorInfo = doctor,
                GlobalStatistics = stats,
                DoctorStatisticsInfo = doctorStats,
                TodayAppointmentsList = todayAppointments,
                UpcomingAppointmentsList = upcomingAppointments,
                UrgentCasesList = urgentCases,
                MyPatientsList = myPatients,
                AvailableDoctorsList = availableDoctors,
                AppointmentRequestsList = appointmentRequests
            };

            ViewData["Title"] = "Tableau de Bord Médecin";
            ViewBag.DoctorName = doctor.Name;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAvailability([FromBody] DoctorAvailabilityRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
                return Json(new { success = false, message = "Médecin non trouvé" });

            doctor.IsAvailable = request.IsAvailable;
            doctor.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _notificationService.SendDoctorStatusUpdate(doctor.Id, request.IsAvailable);

            return Json(new { success = true, isAvailable = doctor.IsAvailable });
        }

        [HttpPost]
        public async Task<IActionResult> CompleteAppointment(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return Json(new { success = false, message = "Rendez-vous non trouvé" });

            appointment.Status = "Completed";
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _notificationService.SendAppointmentCompletion(appointment.PatientId, appointment.Id);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, string status)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return Json(new { success = false, message = "Rendez-vous non trouvé" });

            appointment.Status = status;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true, newStatus = status });
        }

        [HttpGet]
        public async Task<IActionResult> GetTodaySchedule()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
                return Json(new { success = false, message = "Médecin non trouvé" });

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctor.Id && a.Date.Date == DateTime.UtcNow.Date)
                .OrderBy(a => a.StartTime)
                .Select(a => new
                {
                    a.Id,
                    PatientName = a.Patient.Name,
                    PatientAge = a.Patient.Age,
                    PatientGender = a.Patient.Gender,
                    Time = a.StartTime.HasValue ? a.StartTime.Value.ToString(@"hh\:mm") : "",
                    EndTime = a.EndTime.HasValue ? a.EndTime.Value.ToString(@"hh\:mm") : "",
                    Reason = a.Reason,
                    Status = a.Status,
                    Duration = a.EndTime.HasValue && a.StartTime.HasValue
                        ? (a.EndTime.Value - a.StartTime.Value).TotalMinutes
                        : 30,
                    Notes = a.Notes
                })
                .ToListAsync();

            return Json(new { success = true, appointments });
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientDetails(int patientId)
        {
            var patient = await _context.Patients
                .Include(p => p.TriageResults)
                .Include(p => p.Symptoms)
                .Include(p => p.MedicalRecords)
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (patient == null)
                return Json(new { success = false, message = "Patient non trouvé" });

            var patientData = new
            {
                patient.Id,
                patient.Name,
                patient.Age,
                patient.Gender,
                patient.BloodType,
                patient.Allergies,
                patient.MedicalHistory,
                RecentTriage = patient.TriageResults
                    .OrderByDescending(tr => tr.CreatedAt)
                    .Take(3)
                    .Select(tr => new
                    {
                        tr.Level,
                        tr.Score,
                        tr.Recommendation,
                        Date = tr.CreatedAt.ToString("dd/MM/yyyy")
                    }),
                RecentSymptoms = patient.Symptoms
                    .OrderByDescending(s => s.Date)
                    .Take(5)
                    .Select(s => new
                    {
                        s.Name,
                        s.Severity,
                        s.Description,
                        Date = s.Date.ToString("dd/MM/yyyy")
                    }),
                MedicalRecordsCount = patient.MedicalRecords.Count
            };

            return Json(new { success = true, patient = patientData });
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
                return Json(new { success = false });

            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek + 1);

            var statistics = new
            {
                AppointmentsToday = await _context.Appointments
                    .CountAsync(a => a.DoctorId == doctor.Id && a.Date.Date == today),
                AppointmentsThisWeek = await _context.Appointments
                    .CountAsync(a => a.DoctorId == doctor.Id && a.Date >= weekStart),
                PatientsThisMonth = await _context.Appointments
                    .Where(a => a.DoctorId == doctor.Id && a.Date >= new DateTime(today.Year, today.Month, 1))
                    .Select(a => a.PatientId)
                    .Distinct()
                    .CountAsync(),
                UrgentCases = await _context.TriageResults
                    .CountAsync(tr => tr.DoctorId == doctor.Id && tr.Level == "Urgent"),
                AverageRating = 4.7m,
                Revenue = 12540.75m
            };

            return Json(new { success = true, statistics });
        }

        [HttpPost]
        public async Task<IActionResult> AddPrescription([FromBody] DoctorPrescriptionRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
                return Json(new { success = false, message = "Médecin non trouvé" });

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId && a.DoctorId == doctor.Id);

            if (appointment == null)
                return Json(new { success = false, message = "Rendez-vous non trouvé" });

            appointment.Prescription = request.Prescription;
            appointment.Diagnosis = request.Diagnosis;
            appointment.FollowUpInstructions = request.FollowUpInstructions;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _notificationService.SendPrescriptionNotification(appointment.PatientId, appointment.Id);

            return Json(new { success = true, message = "Ordonnance ajoutée avec succès" });
        }

        private async Task<List<Patient>> GetMyPatients(int doctorId, int count = 10)
        {
            var patientIds = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => a.PatientId)
                .Distinct()
                .ToListAsync();

            return await _context.Patients
                .Where(p => patientIds.Contains(p.Id))
                .OrderByDescending(p => p.UpdatedAt)
                .Take(count)
                .ToListAsync();
        }
    }

    // Classes locales pour les requêtes
    public class DoctorAvailabilityRequest
    {
        public bool IsAvailable { get; set; }
    }

    public class DoctorPrescriptionRequest
    {
        public int AppointmentId { get; set; }
        public string Prescription { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string FollowUpInstructions { get; set; } = string.Empty;
    }
}