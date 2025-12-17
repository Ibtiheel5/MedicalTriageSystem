using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalTriageSystem.Controllers
{
    public class TriageResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TriageResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var results = await _context.TriageResults
                .Include(tr => tr.Patient)
                .Include(tr => tr.Doctor)
                .OrderByDescending(tr => tr.CreatedAt)
                .ToListAsync();

            return View(results);
        }
    }
}