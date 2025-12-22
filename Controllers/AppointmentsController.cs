using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalTriageSystem.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderBy(a => a.ScheduledDate) // ✅ Utiliser ScheduledDate au lieu de Date
                .ThenBy(a => a.StartTime)
                .ToListAsync();

            return View(appointments);
        }

        // Supprimer async ici car il n'y a pas d'await
        public IActionResult Calendar()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Select(a => new
                {
                    id = a.Id,
                    title = $"{a.Patient.Name} - {a.Doctor.Name}",
                    start = a.ScheduledDate.ToString("yyyy-MM-dd") + "T" + a.StartTime.ToString(@"hh\:mm\:ss"), // ✅ Utiliser ScheduledDate
                    end = a.ScheduledDate.ToString("yyyy-MM-dd") + "T" + a.EndTime.ToString(@"hh\:mm\:ss"), // ✅ Utiliser ScheduledDate
                    color = GetAppointmentColor(a.Status),
                    extendedProps = new
                    {
                        patient = a.Patient.Name,
                        doctor = a.Doctor.Name,
                        reason = a.Reason,
                        status = a.Status
                    }
                })
                .ToListAsync();

            return Json(appointments);
        }

        private string GetAppointmentColor(string status)
        {
            return status switch
            {
                "Scheduled" => "#3182ce",
                "Completed" => "#38a169",
                "Cancelled" => "#e53e3e",
                _ => "#a0aec0"
            };
        }
    }
}