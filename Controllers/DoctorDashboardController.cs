using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using MedicalTriageSystem.Models.ViewModels;
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

        public DoctorDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var currentUserId = userIdClaim != null && int.TryParse(userIdClaim.Value, out int id) ? id : 0;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == currentUserId);

            var stats = new DashboardStatistics
            {
                TotalPatients = await _context.Patients.CountAsync(),
                TotalDoctors = await _context.Doctors.CountAsync(),
                TotalTriages = await _context.TriageResults.CountAsync(),
                UrgentCases = await _context.TriageResults.CountAsync(tr => tr.Level == "Urgent")
            };

            var recentPatients = await _context.Patients
                .OrderByDescending(p => p.CreatedAt)
                .Take(10)
                .ToListAsync();  // ✅ Sans LicenseNumber

            var urgentCases = await _context.TriageResults
                .Include(tr => tr.Patient)
                .Where(tr => tr.Level == "Urgent")
                .OrderByDescending(tr => tr.CreatedAt)
                .Take(5)
                .ToListAsync();

            var upcomingAppointments = doctor != null
                ? await _context.Appointments
                    .Include(a => a.Patient)
                    .Where(a => a.DoctorId == doctor.Id && a.Date >= DateTime.Today)
                    .OrderBy(a => a.Date).ThenBy(a => a.StartTime)
                    .Take(10)
                    .ToListAsync()
                : new List<Appointment>();

            var viewModel = new DashboardViewModel
            {
                Statistics = stats,
                RecentPatients = recentPatients,
                UrgentCases = urgentCases,
                DoctorAvailability = await _context.Doctors
                    .OrderBy(d => d.IsAvailable ? 0 : 1)
                    .ThenBy(d => d.Name)
                    .Take(5)
                    .Select(d => new Doctor  // ✅ Projection sans LicenseNumber
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Specialty = d.Specialty,
                        IsAvailable = d.IsAvailable
                    })
                    .ToListAsync(),
                UpcomingAppointments = upcomingAppointments
            };

            ViewData["Title"] = "Dashboard Médecin";
            return View(viewModel);
        }
    }
}