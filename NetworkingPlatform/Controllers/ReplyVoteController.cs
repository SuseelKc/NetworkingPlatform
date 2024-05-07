using Microsoft.AspNetCore.Mvc;
using NetworkingPlatform.Data;

namespace NetworkingPlatform.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ReplyVoteController: ControllerBase
    {
        private readonly AppDbContext _context;

        public ReplyVoteController(AppDbContext context)
        {
            _context = context;
        }
    }
}
