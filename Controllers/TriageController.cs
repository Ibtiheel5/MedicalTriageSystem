using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalTriageSystem.Controllers
{
    public class TriageController : Controller
    {
        [AllowAnonymous] // Accessible sans authentification
        public IActionResult Quick()
        {
            // Version simplifiée du triage pour les utilisateurs non authentifiés
            return View();
        }

        [Authorize] // Nécessite l'authentification
        public IActionResult Index()
        {
            // Version complète du triage
            return View();
        }

        [Authorize] // Nécessite l'authentification
        public IActionResult Create()
        {
            return View();
        }

        [Authorize] // Nécessite l'authentification
        [HttpPost]
        public IActionResult Create(TriageModel model)
        {
            // Logique de création de triage
            return RedirectToAction("Results", new { id = model.Id });
        }

        [Authorize] // Nécessite l'authentification
        public IActionResult Results(int id)
        {
            return View();
        }
    }

    // Modèle pour le triage
    public class TriageModel
    {
        public int Id { get; set; }
        public string Symptoms { get; set; }
        public string Severity { get; set; }
        // ... autres propriétés
    }
}