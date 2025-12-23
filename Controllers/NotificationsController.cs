using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalTriageSystem.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var notifications = await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Patient)
                .Include(n => n.Doctor)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return View(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var notifications = await _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(10)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    n.Type,
                    n.IsRead,
                    CreatedAt = n.CreatedAt.ToString("dd/MM/yyyy HH:mm") // Formatage
                })
                .ToListAsync();
            return Json(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow; // ✅ CORRECTION: Utilisez UtcNow
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var notifications = await _context.Notifications
                .Where(n => !n.IsRead)
                .ToListAsync();
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow; // ✅ CORRECTION: Utilisez UtcNow
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ClearAll()
        {
            var notifications = await _context.Notifications.ToListAsync();
            _context.Notifications.RemoveRange(notifications);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Méthode pour créer une notification
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Notification notification)
        {
            if (notification == null)
                return BadRequest("Notification invalide");

            notification.CreatedAt = DateTime.UtcNow; // ✅ CORRECTION: Utilisez UtcNow
            notification.IsRead = false;

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Json(new { success = true, id = notification.Id });
        }
    }
}