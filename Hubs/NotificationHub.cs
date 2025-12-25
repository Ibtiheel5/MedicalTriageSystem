using Microsoft.AspNetCore.SignalR;

namespace MedicalTriageSystem.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string userId, string message, string type)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message, type);
        }

        public async Task JoinDoctorGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Doctors");
        }

        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}