using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalTriageSystem.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        public IActionResult Dashboard()
        {
            return RedirectToAction("Dashboard", "PatientDashboard");
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }
    }
}