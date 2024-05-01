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
    public class UpvotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UpvotesController(AppDbContext context)
        {
            _context = context;
        }


        //[HttpPost]
        //[Route("UpvotePost")]
        //public async Task<IActionResult> addUpvote(Upvotes upvote, Posts post)
        //{// Get the user ID of the logged-in user
        //    var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (userId == null)
        //    {
        //        return Unauthorized(); // User is not authenticated
        //    }

        //    // Check if there's an existing upvote for the same post by the same user
        //    var existingDownvote = await _context.Downvotes
        //        .FirstOrDefaultAsync(u => u.post_id == post.ID && u.users_id == userId);

        //    if (existingDownvote != null)
        //    {
        //        // Remove the existing upvote
        //        _context.Downvotes.Remove(existingDownvote);
        //    }

        //    // Add the downvote
        //    upvote.users_id = userId; // Set the user ID
        //    upvote.post_id = post.ID; //
        //    _context.Upvotes.Add(upvote);
        //    await _context.SaveChangesAsync();
        //    return Ok("Upvote Post Successfully!");
        //}

        //[HttpDelete]
        //[Route("deleteUpvote")]
        //public string deleteUpvote(int id)
        //{

        //    Upvotes upvote = _context.Upvotes.Where(x => x.Id == id).FirstOrDefault();
        //    if (upvote != null)
        //    {
        //        _context.Upvotes.Remove(upvote);
        //        _context.SaveChanges();
        //        return "Upvote deleted Successfullly";

        //    }
        //    else
        //    {
        //        return "no upvote found";
        //    }


        //}

    }
}
