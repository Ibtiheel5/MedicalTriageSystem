using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalTriageSystem.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var patients = await _context.Patients
                .Include(p => p.Symptoms)
                .Include(p => p.TriageResult)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(patients);
        }

        public async Task<IActionResult> Details(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Symptoms)
                .Include(p => p.TriageResult)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }
    }
}