using MedicalTriageSystem.Data;
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

        public async Task<IActionResult> Index()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var patient = await _context.Patients
                .Include(p => p.Symptoms)
                .Include(p => p.TriageResults)
                    .ThenInclude(tr => tr.Doctor)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return RedirectToAction("Login", "Auth");

            var vm = new PatientDashboardViewModel
            {
                Patient = patient,
                TriageHistory = patient.TriageResults
                    .OrderByDescending(t => t.CreatedAt)
                    .ToList(),
                LastTriage = patient.TriageResults
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefault(),
                Symptoms = patient.Symptoms.ToList(),
                Appointments = await _context.Appointments
                    .Where(a => a.PatientId == patient.Id)
                    .Include(a => a.Doctor)
                    .OrderByDescending(a => a.ScheduledDate) // ✅ Utiliser ScheduledDate
                    .ToListAsync()
            };

            return View(vm);
        }
    }
}