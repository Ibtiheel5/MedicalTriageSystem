using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedicalTriageSystem.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class TriageResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TriageResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string filter = "all", string sort = "newest")
        {
            var query = _context.TriageResults
                .Include(tr => tr.Patient)
                .Include(tr => tr.Doctor)
                .AsQueryable();

            // Apply filters
            switch (filter.ToLower())
            {
                case "urgent":
                    query = query.Where(tr => tr.Level == "Urgent");
                    break;
                case "today":
                    query = query.Where(tr => tr.CreatedAt.Date == DateTime.UtcNow.Date);
                    break;
                case "week":
                    query = query.Where(tr => tr.CreatedAt >= DateTime.UtcNow.AddDays(-7));
                    break;
            }

            // Apply sorting
            switch (sort.ToLower())
            {
                case "score_high":
                    query = query.OrderByDescending(tr => tr.Score);
                    break;
                case "score_low":
                    query = query.OrderBy(tr => tr.Score);
                    break;
                case "oldest":
                    query = query.OrderBy(tr => tr.CreatedAt);
                    break;
                default: // newest
                    query = query.OrderByDescending(tr => tr.CreatedAt);
                    break;
            }

            var results = await query.ToListAsync();
            ViewBag.Filter = filter;
            ViewBag.Sort = sort;

            return View(results);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Review(int id)
        {
            var triageResult = await _context.TriageResults
                .Include(tr => tr.Patient)
                .FirstOrDefaultAsync(tr => tr.Id == id);

            if (triageResult == null) return NotFound();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor != null)
            {
                triageResult.DoctorId = doctor.Id;
                await _context.SaveChangesAsync();
            }

            return View(triageResult);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AddNotes(int id, string notes)
        {
            var triageResult = await _context.TriageResults.FindAsync(id);
            if (triageResult == null) return Json(new { success = false });

            triageResult.Notes = notes;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Export(string format = "json")
        {
            var results = await _context.TriageResults
                .Include(tr => tr.Patient)
                .Select(tr => new
                {
                    tr.Id,
                    Patient = tr.Patient.Name,
                    tr.Score,
                    tr.Level,
                    tr.Recommendation,
                    Date = tr.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                    Doctor = tr.Doctor != null ? tr.Doctor.Name : "Non assigné"
                })
                .ToListAsync();

            if (format.ToLower() == "csv")
            {
                var csv = "ID,Patient,Score,Niveau,Recommandation,Date,Médecin\n";
                csv += string.Join("\n", results.Select(r =>
                    $"{r.Id},{r.Patient},{r.Score},{r.Level},{r.Recommendation},{r.Date},{r.Doctor}"));

                return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", $"triage-results-{DateTime.Now:yyyyMMdd}.csv");
            }

            return Json(results);
        }

        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            var stats = new
            {
                Total = await _context.TriageResults.CountAsync(),
                Urgent = await _context.TriageResults.CountAsync(tr => tr.Level == "Urgent"),
                High = await _context.TriageResults.CountAsync(tr => tr.Level == "Élevé"),
                Medium = await _context.TriageResults.CountAsync(tr => tr.Level == "Moyen"),
                Low = await _context.TriageResults.CountAsync(tr => tr.Level == "Faible"),
                Today = await _context.TriageResults.CountAsync(tr => tr.CreatedAt.Date == DateTime.UtcNow.Date),
                ThisWeek = await _context.TriageResults.CountAsync(tr => tr.CreatedAt >= DateTime.UtcNow.AddDays(-7)),
                AverageScore = await _context.TriageResults.AverageAsync(tr => (double)tr.Score)
            };

            return Json(stats);
        }
    }
}