using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalTriageSystem.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        public IActionResult Dashboard()
        {
            return RedirectToAction("PatientDashboard", "PatientsDashboard");  // Mettre à jour
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }
    }
}