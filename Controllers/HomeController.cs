using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalTriageSystem.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard"); // Dashboard Admin
            }
            else if (User.IsInRole("Doctor"))
            {
                return RedirectToAction("Index", "DoctorDashboard"); // Dashboard Médecin
            }
            else if (User.IsInRole("Patient"))
            {
                return RedirectToAction("Index", "PatientDashboard"); // Dashboard Patient
            }

            return RedirectToAction("Index", "Home");
        }
    }
}