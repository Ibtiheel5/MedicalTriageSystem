using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedicalTriageSystem.Controllers
{
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _context.Patients.OrderBy(p => p.Name).ToListAsync();
            ViewBag.Doctors = await _context.Doctors.OrderBy(d => d.Name).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            // Supprimer les validations d'objets complexes pour éviter les erreurs ModelState
            ModelState.Remove("Patient");
            ModelState.Remove("Doctor");

            if (User.IsInRole("Patient"))
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdString))
                {
                    int userId = int.Parse(userIdString);
                    var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
                    if (patient != null)
                    {
                        appointment.PatientId = patient.Id;
                        ModelState.Remove("PatientId");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                appointment.CreatedAt = DateTime.Now;
                _context.Add(appointment);
                await _context.SaveChangesAsync();

                // CORRECTION DE LA REDIRECTION :
                // Si l'utilisateur est un patient, on ne le renvoie PAS vers le DoctorDashboard
                if (User.IsInRole("Patient"))
                {
                    // Redirigez vers l'historique de triage ou une page d'accueil patient
                    return RedirectToAction("History", "Triage");
                }

                return RedirectToAction("Index");
            }

            ViewBag.Patients = await _context.Patients.OrderBy(p => p.Name).ToListAsync();
            ViewBag.Doctors = await _context.Doctors.OrderBy(d => d.Name).ToListAsync();
            return View(appointment);
        }

        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderBy(a => a.Date)
                .ToListAsync();

            return View(appointments);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // On récupère le rendez-vous en incluant les données du Patient et du Docteur
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                // Si l'ID n'existe pas en base, on affiche une page 404 propre
                return NotFound();
            }

            // Sécurité : Un patient ne peut voir que SES détails
            if (User.IsInRole("Patient"))
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (appointment.Patient?.UserId != userId)
                {
                    return Forbid(); // Accès refusé si ce n'est pas son RDV
                }
            }

            return View(appointment);
        }
    }
}