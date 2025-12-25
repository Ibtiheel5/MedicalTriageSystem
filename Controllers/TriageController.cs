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
    [Authorize(Roles = "Patient,Doctor")]
    public class TriageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public TriageController(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // Action par défaut : redirige selon le rôle du compte connecté
        [HttpGet]
        public IActionResult Index()
        {
            if (User.IsInRole("Doctor"))
                return RedirectToAction("History");

            return RedirectToAction("Create");
        }

        // Affiche le formulaire de triage (Réservé au Patient)
        [HttpGet]
        [Authorize(Roles = "Patient")]
        public IActionResult Create()
        {
            return View(new QuickTriageRequestViewModel { Symptoms = new List<string>() });
        }

        // Traitement des données envoyées par le formulaire
        [HttpPost]
        [Authorize(Roles = "Patient")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuickTriageRequestViewModel request)
        {
            // Vérification si au moins un symptôme est coché
            if (request.Symptoms == null || !request.Symptoms.Any())
            {
                ModelState.AddModelError("Symptoms", "Veuillez sélectionner au moins un symptôme pour l'analyse.");
            }

            if (!ModelState.IsValid) return View(request);

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Auth");

            int userId = int.Parse(userIdString);

            // On récupère le patient associé à l'utilisateur connecté
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            // Création automatique du profil patient si c'est sa première fois
            if (patient == null)
            {
                var user = await _context.Users.FindAsync(userId);
                patient = new Patient
                {
                    UserId = userId,
                    Name = user?.Username ?? "Patient Anonyme",
                    Email = user?.Email ?? "non-renseigne@trimed.com",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            // Calcul du score et détermination de la sévérité
            int score = CalculateTriageScore(request.Symptoms);
            string level = GetTriageLevel(score);

            var triageResult = new TriageResult
            {
                PatientId = patient.Id,
                Score = score,
                Level = level,
                Recommendation = GetRecommendation(level),
                Symptoms = string.Join(", ", request.Symptoms),
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.TriageResults.Add(triageResult);
            await _context.SaveChangesAsync();

            // Notification immédiate si le cas est critique
            if (level == "Urgent")
            {
                await _notificationService.SendEmergencyAlert(patient.Id, level, triageResult.Recommendation);
            }

            return RedirectToAction("Result", new { id = triageResult.Id });
        }

        // Affiche les détails d'un résultat spécifique
        [HttpGet]
        public async Task<IActionResult> Result(int id)
        {
            var result = await _context.TriageResults
                .Include(tr => tr.Patient)
                .Include(tr => tr.Doctor)
                .FirstOrDefaultAsync(tr => tr.Id == id);

            if (result == null) return NotFound();

            // Sécurité : Un patient ne peut pas voir le résultat d'un autre
            if (User.IsInRole("Patient"))
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (result.Patient?.UserId != currentUserId) return Forbid();
            }

            return View(result);
        }

        // Liste de tous les triages (Filtrée pour patient, Globale pour docteur)
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Auth");

            int userId = int.Parse(userIdString);

            var query = _context.TriageResults
                .Include(tr => tr.Patient)
                .AsQueryable();

            if (User.IsInRole("Patient"))
            {
                // Le patient ne voit que ses données
                query = query.Where(tr => tr.Patient.UserId == userId);
            }
            // Le Docteur voit l'intégralité de la base pour le suivi médical

            var results = await query.OrderByDescending(tr => tr.CreatedAt).ToListAsync();
            return View(results);
        }

        // --- MÉTHODES PRIVÉES DE CALCUL MÉDICAL ---

        private int CalculateTriageScore(List<string> symptoms)
        {
            if (symptoms == null) return 0;

            // Attribution de poids par symptôme
            int totalScore = symptoms.Sum(s => s.ToLower() switch {
                "douleur thoracique" => 40,
                "difficulté respiratoire" => 35,
                "perte de conscience" => 45,
                "confusion" => 25,
                "fièvre élevée" => 20,
                "saignement abondant" => 30,
                _ => 10
            });

            return Math.Min(totalScore, 100); // Capé à 100%
        }

        private string GetTriageLevel(int score) => score switch
        {
            >= 75 => "Urgent",
            >= 40 => "Moyen",
            _ => "Faible"
        };

        private string GetRecommendation(string level) => level switch
        {
            "Urgent" => "ALERTE : Votre état nécessite une attention immédiate. Veuillez contacter le 15 ou vous rendre aux urgences les plus proches.",
            "Moyen" => "Prenez rendez-vous avec votre médecin traitant dans les prochaines 24 heures pour une consultation.",
            _ => "Reposez-vous, surveillez vos symptômes et hydratez-vous. Si l'état persiste, consultez un médecin."
        };
    }
}