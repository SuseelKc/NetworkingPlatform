using Microsoft.AspNetCore.SignalR;

namespace NetworkingPlatform.Hubs
{
    public class NotificationHub:Hub
    {
        public async Task ReceiveNotification(string message)
        {
            await Clients.All.SendAsync("notification", message);
        }

    }
}
