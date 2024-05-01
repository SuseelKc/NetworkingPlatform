using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Data;
using NetworkingPlatform.Models;
using System.Security.Claims;

namespace NetworkingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownvotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DownvotesController(AppDbContext context) {
            _context = context;
        }

        [HttpPost]
        [Route("DownvotePost")]
        public async Task<IActionResult> addDownvote(Downvotes downvote,Posts post)
        {
            // Get the user ID of the logged-in user
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(); // User is not authenticated
            }

            // Check if there's an existing upvote for the same post by the same user
            var existingUpvote = await _context.Upvotes
                .FirstOrDefaultAsync(u => u.post_id == post.ID && u.users_id == userId);

            if (existingUpvote != null)
            {
                // Remove the existing upvote
                _context.Upvotes.Remove(existingUpvote);
            }

            // Add the downvote
            downvote.users_id = userId; // Set the user ID
            downvote.post_id = post.ID; // Set the post ID
            _context.Downvotes.Add(downvote);

            await _context.SaveChangesAsync(); // Save changes asynchronously

            return Ok("Downvote Post Successfully!");
        }

        [HttpDelete]
        [Route("deleteDownvote")]
        public string deleteDownvote(int id)
        {

            Downvotes downvote = _context.Downvotes.Where(x => x.Id == id).FirstOrDefault();
            if (downvote != null)
            {
                _context.Downvotes.Remove(downvote);
                _context.SaveChanges();
                return "downvote deleted Successfullly";

            }
            else
            {
                return "no downvote found";
            }


        }
    }
}
