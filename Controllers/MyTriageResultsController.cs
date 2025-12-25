using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedicalTriageSystem.Controllers
{
    [Authorize(Roles = "Patient")]
    public class MyTriageResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MyTriageResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return NotFound();

            var results = await _context.TriageResults
                .Where(tr => tr.PatientId == patient.Id)
                .OrderByDescending(tr => tr.CreatedAt)
                .ToListAsync();

            ViewData["Title"] = "Mes Résultats de Triage";
            return View(results);
        }
    }
}