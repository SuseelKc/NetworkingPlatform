using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Data;
using NetworkingPlatform.Hubs;
using NetworkingPlatform.Migrations;
using NetworkingPlatform.Models;

namespace NetworkingPlatform.Controllers
{
    [Route("api/")]
    [ApiController]
    public class NotificationController: ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;

        }
        [HttpGet]
        [Route("send")]
        public IActionResult sendNotification(string message)
        {
            _hubContext.Clients.All.SendAsync("RecieveMessage", "Message");
            return Ok("done");

        }

    }
}
