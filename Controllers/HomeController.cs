using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalTriageSystem.Controllers
{
    [AllowAnonymous] // Permet l'accès sans authentification
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

        [Authorize] // Nécessite l'authentification
        public IActionResult Dashboard()
        {
            // Rediriger vers le dashboard approprié selon le rôle
            if (User.IsInRole("Admin") || User.IsInRole("Doctor"))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            else if (User.IsInRole("Patient"))
            {
                return RedirectToAction("Dashboard", "Patient");
            }

            return RedirectToAction("Index");
        }
    }
}