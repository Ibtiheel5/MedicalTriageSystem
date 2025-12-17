using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MedicalTriageSystem.Controllers
{
    public class TriageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TriageController> _logger;

        public TriageController(ApplicationDbContext context, ILogger<TriageController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Patient patient, string symptomsData, int triageScore)
        {
            if (!ModelState.IsValid)
            {
                return View(patient);
            }

            try
            {
                // Sauvegarder le patient
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                // Traiter les symptômes
                if (!string.IsNullOrEmpty(symptomsData))
                {
                    var symptomDataList = JsonSerializer.Deserialize<List<SymptomData>>(symptomsData);
                    if (symptomDataList != null)
                    {
                        foreach (var symptomData in symptomDataList)
                        {
                            var symptom = new Symptom
                            {
                                Name = symptomData.Name,
                                Description = GetSymptomDescription(symptomData.Name),
                                Severity = symptomData.Severity,
                                Category = GetSymptomCategory(symptomData.Name),
                                Date = DateTime.Now,
                                PatientId = patient.Id
                            };
                            _context.Symptoms.Add(symptom);
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                // Déterminer le niveau de triage
                var (level, recommendation) = CalculateTriageLevel(triageScore);

                // Trouver un médecin disponible
                var availableDoctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.IsAvailable);

                // Créer le résultat du triage
                var triageResult = new TriageResult
                {
                    PatientId = patient.Id,
                    Level = level,
                    Recommendation = recommendation,
                    Score = triageScore,
                    CreatedAt = DateTime.Now,
                    DoctorId = availableDoctor?.Id
                };
                _context.TriageResults.Add(triageResult);
                await _context.SaveChangesAsync();

                // Préparer les données pour la vue
                ViewBag.PatientName = patient.Name;
                ViewBag.Level = level;
                ViewBag.Recommendation = recommendation;
                ViewBag.Score = triageScore;
                ViewBag.ScorePercentage = Math.Min(triageScore, 100);
                ViewBag.Doctor = availableDoctor;

                return View("Result");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement du triage");
                ModelState.AddModelError("", "Une erreur est survenue. Veuillez réessayer.");
                return View(patient);
            }
        }

        private (string Level, string Recommendation) CalculateTriageLevel(int score)
        {
            if (score >= 70)
            {
                return ("Urgent",
                    "⚠️ URGENCE MÉDICALE - Consultez immédiatement un médecin ou rendez-vous aux urgences. " +
                    "Ne prenez pas le volant vous-même. Appelez le 15 si nécessaire.");
            }
            else if (score >= 40)
            {
                return ("Élevé",
                    "📞 Consultation recommandée dans les 24 heures. " +
                    "Surveillez vos symptômes et consultez un médecin rapidement.");
            }
            else if (score >= 20)
            {
                return ("Normal",
                    "🏥 Prenez rendez-vous avec votre médecin dans les prochains jours. " +
                    "Reposez-vous et surveillez l'évolution des symptômes.");
            }
            else
            {
                return ("Faible",
                    "💊 Soins à domicile recommandés. " +
                    "Reposez-vous, hydratez-vous et surveillez vos symptômes. " +
                    "Consultez si aggravation.");
            }
        }

        private string GetSymptomCategory(string symptomName)
        {
            var urgentSymptoms = new[] { "Douleur thoracique", "Difficulté respiratoire", "Saignement important" };
            return urgentSymptoms.Contains(symptomName) ? "Urgent" : "Normal";
        }

        private string GetSymptomDescription(string symptomName)
        {
            return symptomName switch
            {
                "Douleur thoracique" => "Douleur ou pression dans la poitrine",
                "Difficulté respiratoire" => "Essoufflement ou respiration difficile",
                "Fièvre élevée" => "Température supérieure à 38.5°C",
                "Saignement important" => "Saignement qui ne s'arrête pas",
                "Maux de tête sévères" => "Céphalées intenses ou persistantes",
                "Nausées/Vomissements" => "Nausées persistantes ou vomissements",
                _ => "Symptôme général"
            };
        }

        // Classe interne pour la désérialisation
        private class SymptomData
        {
            public string Name { get; set; } = string.Empty;
            public int Severity { get; set; }
        }
    }
}