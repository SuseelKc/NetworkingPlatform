using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Data;
using NetworkingPlatform.Migrations;
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


        [HttpGet]
        [Route("GetPost")]
        public IActionResult GetPost(int id)
        {
            try
            {
                Posts post = _context.Posts.Where(x => x.ID == id).FirstOrDefault();

                if (post == null)
                {
                    return NotFound(); // Return 404 Not Found if no posts are found
                }

                return Ok(post); // Return 200 OK with the list of posts
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                // logger.LogError(ex, "An error occurred while retrieving posts.");

                return StatusCode(500, "An error occurred while retrieving posts. Please try again later."); // Return 500 Internal Server Error
            }
        }


        [HttpPut]
        [Route("UpdatePost")]
        public IActionResult UpdatePost(Posts post)
        {
            try
            {
                // Check if the post with the given ID exists in the database
                var existingPost = _context.Posts.FirstOrDefault(p => p.ID == post.ID);
                if (existingPost == null)
                {
                    return NotFound("Post not found"); // Return 404 Not Found if the post doesn't exist
                }

                // Update only specific properties of the post
                _context.Entry(existingPost).CurrentValues.SetValues(post);
                _context.SaveChanges();

                return Ok("Post updated successfully"); // Return 200 OK if the update is successful
            }
           
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, "An error occurred while updating the post. Please try again later.");
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
