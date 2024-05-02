using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Data;
using NetworkingPlatform.Enums;
using NetworkingPlatform.Models;
using System.Security.Claims;

namespace NetworkingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VotesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("UpvotePost/{postId}")]
        public async Task<IActionResult> addUpvote(int postId, [FromBody] Votes vote)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if (userId == null)
            {
                return Unauthorized(); // User is not authenticated
            }

            // Check if there's an existing upvote for the same post by the same user
            var existingVote = await _context.Votes
                .FirstOrDefaultAsync(u => u.post_id == postId && u.users_id == userId);

            if (existingVote != null)
            {
                existingVote.voteType = 0; // Assuming 0 represents an upvote
                await _context.SaveChangesAsync();
                return Ok("Upvote Post Successfully!");
            }

            // Add the upvote
            vote.voteType = 0; // Assuming 0 represents an upvote
            vote.users_id = userId; // Set the user ID
            vote.post_id = postId; // Set the post ID
            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();
            return Ok("Upvote Post Successfully!");
        }



        [HttpPost]
        [Route("DownvotePost/{postId}")]
        public async Task<IActionResult> addDownvote(int postId, [FromBody] Votes vote)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if (userId == null)
            {
                return Unauthorized(); // User is not authenticated
            }

            // Check if there's an existing upvote for the same post by the same user
            var existingVote = await _context.Votes
                .FirstOrDefaultAsync(u => u.post_id == postId && u.users_id == userId);

            if (existingVote != null)
            {
                existingVote.voteType = (Enums.VoteType)1; // Assuming 0 represents an upvote
                await _context.SaveChangesAsync();
                return Ok("Downvote Post Successfully!");
            }

            // Add the upvote
            vote.voteType = (Enums.VoteType)1; // Assuming 0 represents an upvote
            vote.users_id = userId; // Set the user ID
            vote.post_id = postId; // Set the post ID
            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();
            return Ok("Downvote Post Successfully!");
        }


        [HttpDelete]
        [Route("deleteUpVote")]
        public string deleteUpVote(int id)
        {

            Votes vote = _context.Votes.FirstOrDefault(x => x.Id == id && x.voteType == VoteType.Upvote);

            if (vote != null)
            {
                _context.Votes.Remove(vote);
                _context.SaveChanges();
                return "Vote deleted Successfullly";

            }
            else
            {
                return "No Vote found";
            }


        }

        [HttpDelete]
        [Route("deleteDownVote")]
        public string deleteDownVote(int id)
        {

            Votes vote = _context.Votes.FirstOrDefault(x => x.Id == id && x.voteType == VoteType.Downvote);

            if (vote != null)
            {
                _context.Votes.Remove(vote);
                _context.SaveChanges();
                return "Vote deleted Successfullly";

            }
            else
            {
                return "No Vote found";
            }


        }




    }
}
