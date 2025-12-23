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
    public class PatientDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Redirection depuis /Patient/Dashboard ou /Patient/Index
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        /// <summary>
        /// Action principale : Affiche le dashboard du patient connecté
        /// Utilise le même style que le dashboard admin (cartes, tableaux, etc.)
        /// </summary>
        public async Task<IActionResult> Dashboard()
        {
            // Récupérer l'ID de l'utilisateur connecté
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["ErrorMessage"] = "Erreur d'authentification. Veuillez vous reconnecter.";
                return RedirectToAction("Login", "Auth");
            }

            // Charger le patient avec toutes les données nécessaires
            var patient = await _context.Patients
                .Include(p => p.TriageResults)
                .Include(p => p.Symptoms)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            // Si le patient n'existe pas encore (première connexion), le créer automatiquement
            if (patient == null)
            {
                patient = new Patient
                {
                    UserId = userId,
                    Name = User.Identity?.Name ?? "Patient Anonyme",
                    Email = User.FindFirst(ClaimTypes.Email)?.Value ?? "",
                    Phone = User.FindFirst(ClaimTypes.MobilePhone)?.Value,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            // === Calcul des statistiques personnelles ===
            var totalTriages = patient.TriageResults?.Count ?? 0;
            var urgentCasesCount = patient.TriageResults?.Count(tr => tr.Level == "Urgent") ?? 0;
            var todayTriages = patient.TriageResults?
                .Count(tr => tr.CreatedAt.Date == DateTime.UtcNow.Date) ?? 0;
            var weekTriages = patient.TriageResults?
                .Count(tr => tr.CreatedAt >= DateTime.UtcNow.AddDays(-7)) ?? 0;

            var stats = new DashboardStatistics
            {
                TotalTriages = totalTriages,
                UrgentCases = urgentCasesCount,
                TodayTriages = todayTriages,
                WeekTriages = weekTriages,
                AvgTriageTime = TimeSpan.Zero // Peut être calculé plus tard si besoin
            };

            // === Cas urgents personnels (les 5 derniers) ===
            var urgentCases = patient.TriageResults?
                .Where(tr => tr.Level == "Urgent")
                .OrderByDescending(tr => tr.CreatedAt)
                .Take(5)
                .ToList() ?? new List<TriageResult>();

            // === Médecins disponibles (commun à tous, comme dans le dashboard admin) ===
            var doctorAvailability = await _context.Doctors
                .Where(d => d.IsAvailable)
                .OrderBy(d => d.Name)
                .Take(5)
                .Select(d => new Doctor
                {
                    Id = d.Id,
                    Name = d.Name,
                    Specialty = d.Specialty,
                    IsAvailable = d.IsAvailable
                })
                .ToListAsync();

            // === Construction du ViewModel ===
            var viewModel = new PatientDashboardViewModel
            {
                Patient = patient,
                LastTriage = patient.TriageResults?
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefault(),

                UpcomingAppointments = patient.Appointments?
                    .Where(a => a.Date >= DateTime.UtcNow.Date)
                    .OrderBy(a => a.Date)
                    .ThenBy(a => a.StartTime)
                    .Take(5)
                    .ToList() ?? new List<Appointment>(),

                RecentSymptoms = patient.Symptoms?
                    .OrderByDescending(s => s.Date)
                    .Take(10)
                    .ToList() ?? new List<Symptom>(),

                // Données pour le style "admin-like"
                Statistics = stats,
                UrgentCases = urgentCases,
                DoctorAvailability = doctorAvailability
            };

            ViewData["Title"] = "Mon Tableau de Bord";

            return View(viewModel);
        }
    }
}