using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MedicalTriageSystem.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public async Task SendNotification(string title, string message, string type)
        {
            var notification = new NotificationMessage
            {
                Title = title ?? string.Empty,
                Message = message ?? string.Empty,
                Type = type ?? "info",
                CreatedAt = DateTime.Now
            };
            await Clients.All.SendAsync("ReceiveNotification", notification);
        }
    }

    public class NotificationMessage
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "info";
        public DateTime CreatedAt { get; set; }
    }
}