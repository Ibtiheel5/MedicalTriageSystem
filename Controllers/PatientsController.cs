using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalTriageSystem.Controllers
{
    [Authorize(Roles = "Admin,Doctor")] // Seuls les admins et médecins voient la liste
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action pour afficher la liste des patients
        public async Task<IActionResult> Index()
        {
            // On récupère la liste des patients depuis la base de données
            var patients = await _context.Patients.ToListAsync();
            return View(patients);
        }
        // Controllers/PatientsController.cs

        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.User) // Si vous avez besoin des infos du compte
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }
    }
}