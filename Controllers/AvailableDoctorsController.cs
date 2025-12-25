using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalTriageSystem.Controllers
{
    [Authorize(Roles = "Patient")]
    public class AvailableDoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvailableDoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var doctors = await _context.Doctors
                .Where(d => d.IsAvailable)
                .OrderBy(d => d.Name)
                .ToListAsync();

            ViewData["Title"] = "Médecins Disponibles";
            return View(doctors);
        }
    }
}