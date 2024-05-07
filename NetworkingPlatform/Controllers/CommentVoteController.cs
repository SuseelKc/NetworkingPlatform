using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Data;
using NetworkingPlatform.Models;

namespace NetworkingPlatform.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CommentVoteController: ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentVoteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("comment/like/{commentId}")]
        public async Task<IActionResult> AddCommentLike(int commentId, [FromBody] CommentVotes vote)
        {
            //var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == vote.users_id);
            if (user == null)
            {
                return StatusCode(400, "Invalid user id");
            }


            // Check if there's an existing upvote for the same post by the same user
            var existingVote = await _context.CommentVotes
                .FirstOrDefaultAsync(u => u.comment_id == commentId && u.users_id == vote.users_id);

            if (existingVote != null)
            {
                existingVote.voteType = existingVote.voteType == 2 ? 1 : existingVote.voteType == 0 ? 1 : 2; // Assuming 2 represents neither like noe unlike
                await _context.SaveChangesAsync();
                return Ok("Success");
            }

            // Add the upvote
            vote.voteType = 1; // Assuming 1 represents an like
            _context.CommentVotes.Add(vote);
            await _context.SaveChangesAsync();
            return Ok("Comment liked");
        }



        [HttpPost]
        [Route("comment/unlike/{commentId}")]
        public async Task<IActionResult> addDownvote(int commentId, [FromBody] CommentVotes vote)
        {
            //var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == vote.users_id);
            if (user == null)
            {
                return StatusCode(400, "Invalid user id");
            }


            // Check if there's an existing upvote for the same post by the same user
            var existingVote = await _context.CommentVotes
                .FirstOrDefaultAsync(u => u.comment_id == commentId && u.users_id == vote.users_id);

            if (existingVote != null)
            {
                existingVote.voteType = existingVote.voteType == 2 ? 0 : existingVote.voteType == 1 ? 0 : 2; // Assuming 2 represents nothing
                await _context.SaveChangesAsync();
                return Ok("Reply unliked Successfully!");
            }

            // Add the upvote
            vote.voteType = 0; // Assuming 0 represents an unlike
            _context.CommentVotes.Add(vote);
            await _context.SaveChangesAsync();
            return Ok("Downvote Post Successfully!");
        }

        [HttpGet]
        [Route("comment/likes/{commentId}")]
        public async Task<IActionResult> getLikes(int commentId)
        {
            try
            {
                var post = await _context.PostComments.FirstOrDefaultAsync(p => p.ID == commentId);
                if (post == null)
                {
                    return StatusCode(400, "No comment found");
                }
                var likes = await _context.CommentVotes.Where(p => p.comment_id == commentId).ToListAsync();

                return Ok(likes); // Return 200 OK with the list of posts
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                // logger.LogError(ex, "An error occurred while retrieving posts.");

                return StatusCode(500, "An error occurred while retrieving posts. Please try again later."); // Return 500 Internal Server Error
            }
        }

    }
}
