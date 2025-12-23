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
                .OrderBy(a => a.Date) // ✅ Utiliser Date (nom de colonne dans la table)
                .ThenBy(a => a.StartTime) // ✅ Utiliser StarTime (nom de colonne)
                .ToListAsync();

            return View(appointments);
        }

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
                    title = a.Doctor != null
                        ? $"{a.Patient.Name} - {a.Doctor.Name}"
                        : a.Patient.Name,
                    start = a.Date.ToString("yyyy-MM-dd") + "T" +
                           (a.StartTime != null ? a.StartTime.Value.ToString(@"hh\:mm\:ss") : "09:00:00"),
                    end = a.Date.ToString("yyyy-MM-dd") + "T" +
                         (a.EndTime != null ? a.EndTime.Value.ToString(@"hh\:mm\:ss") : "09:30:00"),
                    color = GetAppointmentColor(a.Status),
                    extendedProps = new
                    {
                        patient = a.Patient.Name,
                        doctor = a.Doctor != null ? a.Doctor.Name : "Non assigné", // ✅ CORRECTION
                        reason = a.Reason, // ✅ Utiliser Reason (nom de colonne)
                        status = a.Status,
                        notes = a.Notes
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

        // Méthodes supplémentaires
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Patients = _context.Patients.ToList();
            ViewBag.Doctors = _context.Doctors.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.CreatedAt = DateTime.UtcNow;
                appointment.UpdatedAt = DateTime.UtcNow;
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Patients = _context.Patients.ToList();
            ViewBag.Doctors = _context.Doctors.ToList();
            return View(appointment);
        }
    }
}