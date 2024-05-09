using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Data;
using NetworkingPlatform.Enums;
using NetworkingPlatform.Hubs;
using NetworkingPlatform.Models;
using System.Security.Claims;

namespace NetworkingPlatform.Controllers
{
    [Route("api/")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public VotesController(AppDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost]
        [Route("like/{postId}")]
        public async Task<IActionResult> addUpvote(int postId, [FromBody] Votes vote)
        {
            //var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == vote.users_id);
            if (user == null)
            {
                return StatusCode(400, "Invalid user id");
            }
           

            // Check if there's an existing upvote for the same post by the same user
            var existingVote = await _context.Votes
                .FirstOrDefaultAsync(u => u.post_id == postId && u.users_id == vote.users_id);

            if (existingVote != null)
            {
                existingVote.voteType = existingVote.voteType== 2 ? 1 : existingVote.voteType== 0 ? 1 : 2; // Assuming 2 represents neither like noe unlike
                await _context.SaveChangesAsync();
                return Ok("Success");
            }

            // Add the upvote
            vote.voteType = 1; // Assuming 1 represents an like
            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();
            //signal r notification
            await _hubContext.Clients.User(vote.users_id).SendAsync("ReceiveNotification", "You received a new vote!");
            return Ok("Post liked");
        }



        [HttpPost]
        [Route("unlike/{postId}")]
        public async Task<IActionResult> addDownvote(int postId, [FromBody] Votes vote)
        {
            //var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == vote.users_id);
            if (user == null)
            {
                return StatusCode(400, "Invalid user id");
            }
           

            // Check if there's an existing upvote for the same post by the same user
            var existingVote = await _context.Votes
                .FirstOrDefaultAsync(u => u.post_id == postId && u.users_id == vote.users_id);

            if (existingVote != null)
            {
                existingVote.voteType = existingVote.voteType == 2 ? 0 : existingVote.voteType==1 ? 0: 2; // Assuming 2 represents nothing
                await _context.SaveChangesAsync();
                return Ok("Downvote Post Successfully!");
            }

            // Add the upvote
            vote.voteType = 0; // Assuming 0 represents an unlike
            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();
            //signal r notification
            await _hubContext.Clients.User(vote.users_id).SendAsync("ReceiveNotification", "You received a DownVote!");
            return Ok("Downvote Post Successfully!");
        }

        [HttpGet]
        [Route("likes/{postId}")]
        public async Task<IActionResult> getLikes(int postId)
        {
            try
            {
                var post = await _context.Posts.FirstOrDefaultAsync(p=>p.ID == postId);
                if (post == null)
                {
                    return StatusCode(400, "No post found");
                }
                var likes =await _context.Votes.Where(p=> p.post_id==postId).ToListAsync();

                return Ok(likes); // Return 200 OK with the list of posts
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                // logger.LogError(ex, "An error occurred while retrieving posts.");

                return StatusCode(500, "An error occurred while retrieving posts. Please try again later."); // Return 500 Internal Server Error
            }
        }

        [HttpGet]
        [Route("likes/{postId}/users")]
        public async Task<IActionResult> getUsersList(int postId)
        {
            try
            {
                var post = await _context.Posts.FirstOrDefaultAsync(p => p.ID == postId);
                if (post == null)
                {
                    return StatusCode(400, "No post found");
                }
                var likes = await _context.Votes.Where(p => p.post_id == postId).Select(n => new
                {
                    id = n.users_id,
                    avater = "",
                    author = _context.Users.FirstOrDefault(u => u.Id == n.users_id).UserName
                }).ToListAsync();

                return Ok(likes); // Return 200 OK with the list of posts
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                // logger.LogError(ex, "An error occurred while retrieving posts.");

                return StatusCode(500, ex.Message); // Return 500 Internal Server Error
            }
        }


        //[HttpDelete]
        //[Route("deleteUpVote")]
        //public string deleteUpVote(int id)
        //{

        //    Votes vote = _context.Votes.FirstOrDefault(x => x.Id == id && x.voteType == VoteType.Upvote);

        //    if (vote != null)
        //    {
        //        _context.Votes.Remove(vote);
        //        _context.SaveChanges();
        //        return "Vote deleted Successfullly";

        //    }
        //    else
        //    {
        //        return "No Vote found";
        //    }


        //}

        //[HttpDelete]
        //[Route("deleteDownVote")]
        //public string deleteDownVote(int id)
        //{

        //    Votes vote = _context.Votes.FirstOrDefault(x => x.Id == id && x.voteType == VoteType.Downvote);

        //    if (vote != null)
        //    {
        //        _context.Votes.Remove(vote);
        //        _context.SaveChanges();
        //        return "Vote deleted Successfullly";

        //    }
        //    else
        //    {
        //        return "No Vote found";
        //    }


        //}




    }
}
