using MedicalTriageSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalTriageSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            var userRole = User?.Claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            return userRole switch
            {
                "Admin" => RedirectToAction("Index", "Dashboard"),
                "Doctor" => RedirectToAction("Index", "DoctorDashboard"),
                "Patient" => RedirectToAction("Dashboard", "PatientDashboard"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}