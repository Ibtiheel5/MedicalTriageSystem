using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace MedicalTriageSystem.Controllers
{
    public class TriageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TriageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Quick()
        {
            return View();
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(new Patient()); // Model vide pour formulaire
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Patient model, string symptomsData, int triageScore)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                patient = new Patient
                {
                    UserId = userId,
                    Name = User.Identity.Name,
                    Email = User.FindFirstValue(ClaimTypes.Email),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            // Calcul level basé sur score (exemple développé)
            string level = triageScore switch
            {
                > 30 => "Urgent",
                > 20 => "Élevé",
                > 10 => "Normal",
                _ => "Faible"
            };

            var triageResult = new TriageResult
            {
                PatientId = patient.Id,
                Score = triageScore,
                Level = level,
                Recommendation = "Consultez un médecin si symptômes persistent.", // Personnaliser
                CreatedAt = DateTime.UtcNow
            };

            _context.TriageResults.Add(triageResult);
            await _context.SaveChangesAsync();

            ViewBag.Level = level;
            ViewBag.PatientName = patient.Name;
            return View("Result");
        }

        [Authorize]
        public IActionResult Results(int id)
        {
            // Logique pour afficher résultat spécifique
            return View();
        }
    }

    // Model pour triage (ajouté pour complétude)
    public class TriageModel
    {
        public int Id { get; set; }
        public string Symptoms { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
    }
}