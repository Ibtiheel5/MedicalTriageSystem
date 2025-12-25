using MedicalTriageSystem.Models;
using MedicalTriageSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalTriageSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // On utilise AdminDashboardViewModel au lieu de AdminDashboardSimpleViewModel
            var model = new AdminDashboardViewModel
            {
                Statistics = new DashboardStatsViewModel // Notez le changement de nom ici aussi
                {
                    TotalPatients = 150,
                    TotalDoctors = 25,
                    TotalTriages = 500,
                    UrgentCasesCount = 12,
                    TodayTriagesCount = 45,
                    AvgTriageTime = TimeSpan.FromMinutes(15)
                },
                RecentPatients = new List<Patient>(), // Liste vide pour l'instant
                UrgentCasesList = new List<TriageResult>(),
                DoctorAvailability = new List<Doctor>(),
                UpcomingAppointments = new List<Appointment>()
            };

            return View(model);
        }
    }
}