using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetworkingPlatform.Data;
using NetworkingPlatform.Models;
using System.Linq;

namespace NetworkingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostsController(AppDbContext context)
        {
            _context = context;
        }



        [HttpPost]
        [Route("AddPost")]
        public string AddPost(Posts Post)
        {
            _context.Posts.Add(Post);
            _context.SaveChanges();
            return "Post Added Sucessfully!";
        }



        [HttpGet]
        [Route("GetAllPosts")]
        public IActionResult GetPosts()
        {
            try
            {
                var posts = _context.Posts.ToArray();

                if (posts == null || !posts.Any())
                {
                    return NotFound(); // Return 404 Not Found if no posts are found
                }

                return Ok(posts); // Return 200 OK with the list of posts
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                // logger.LogError(ex, "An error occurred while retrieving posts.");

                return StatusCode(500, "An error occurred while retrieving posts. Please try again later."); // Return 500 Internal Server Error
            }
        }



        [HttpDelete]
        [Route("deletePost")]
        public string deletePost(int id)
        {

            Posts post = _context.Posts.Where(x => x.ID == id).FirstOrDefault();
            if (post != null)
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
                return "Post deleted Successfullly";

            }
            else
            {
                return "no Post found";
            }


        }


    }
}
