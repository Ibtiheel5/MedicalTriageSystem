using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using MedicalTriageSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalTriageSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Tableau de bord";

                var stats = await GetStatisticsAsync();
                var recentPatients = await GetRecentPatientsAsync();
                var urgentCases = await GetUrgentCasesAsync();
                var doctors = await GetDoctorAvailabilityAsync();

                var dashboardData = new DashboardViewModel
                {
                    Statistics = stats,
                    RecentPatients = recentPatients,
                    UrgentCases = urgentCases,
                    DoctorAvailability = doctors
                };

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                // En cas d'erreur, retourner une vue avec des données vides
                Console.WriteLine($"Erreur Dashboard: {ex.Message}");
                return View(new DashboardViewModel());
            }
        }

        private async Task<DashboardStatistics> GetStatisticsAsync()
        {
            try
            {
                return new DashboardStatistics
                {
                    TotalPatients = await _context.Patients.CountAsync(),
                    TotalDoctors = await _context.Doctors.CountAsync(),
                    TotalTriages = await _context.TriageResults.CountAsync(),
                    UrgentCases = await _context.TriageResults.CountAsync(tr => tr.Level == "Urgent"),
                    TodayTriages = await _context.TriageResults
                        .CountAsync(tr => tr.CreatedAt.Date == DateTime.Today),
                    WeekTriages = 0, // Temporaire
                    AvgTriageTime = TimeSpan.FromMinutes(15)
                };
            }
            catch
            {
                return new DashboardStatistics();
            }
        }

        private async Task<List<Patient>> GetRecentPatientsAsync()
        {
            try
            {
                return await _context.Patients
                    .Include(p => p.TriageResults)
                        .ThenInclude(tr => tr.Doctor)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(10)
                    .ToListAsync();
            }
            catch
            {
                return new List<Patient>();
            }
        }

        private async Task<List<TriageResult>> GetUrgentCasesAsync()
        {
            try
            {
                return await _context.TriageResults
                    .Include(tr => tr.Patient)
                    .Where(tr => tr.Level == "Urgent")
                    .OrderByDescending(tr => tr.CreatedAt)
                    .Take(5)
                    .ToListAsync();
            }
            catch
            {
                return new List<TriageResult>();
            }
        }

        private async Task<List<Doctor>> GetDoctorAvailabilityAsync()
        {
            try
            {
                return await _context.Doctors
                    .OrderBy(d => d.IsAvailable ? 0 : 1)
                    .ThenBy(d => d.Name)
                    .Take(5)
                    .ToListAsync();
            }
            catch
            {
                return new List<Doctor>();
            }
        }
    }
}