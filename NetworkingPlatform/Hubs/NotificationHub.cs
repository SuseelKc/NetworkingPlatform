using Microsoft.AspNetCore.SignalR;

namespace NetworkingPlatform.Hubs
{
    public class NotificationHub:Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }


        public async Task SendNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
            _logger.LogInformation($"Notification sent to user {userId}: {message}");
        }

    }
}
