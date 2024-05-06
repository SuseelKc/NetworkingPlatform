using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Data;
using NetworkingPlatform.Migrations;
using NetworkingPlatform.Models;
using System.Linq;

namespace NetworkingPlatform.Controllers
{
    [Route("api/")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostsController(AppDbContext context)
        {
            _context = context;
        }



        [HttpPost]
        [Route("post")]
        public async Task<IActionResult> AddPost(Posts Post)
        {
            try
            {
                var user =await _context.Users.FirstOrDefaultAsync(u=>u.Id==Post.users_id);
                if (user == null)
                {
                    return Unauthorized();
                }
                _context.Posts.Add(Post);
                _context.SaveChanges();
                return Ok(Post);
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpGet]
        [Route("posts")]
        public IActionResult GetPosts()
        {
            try
            {
                var posts = _context.Posts.ToList();
                if (posts == null || !posts.Any())
                {
                    return NotFound(); // Return 404 Not Found if no posts are found
                }

                var postsWithUsers = new List<object>(); // Create a list to store posts along with user information
                foreach (var post in posts)
                {
                    var user = _context.Users.Find(post.users_id); // Retrieve user information for the post

                    // Create an anonymous object containing post and user information
                    var postWithUser = new
                    {
                        Post = post,
                        User = user
                    };

                    postsWithUsers.Add(postWithUser); // Add post with user information to the list
                }
                return Ok(postsWithUsers); // Return 200 OK with the list of posts

            }
            catch (Exception ex)
            {
               
                return StatusCode(500, "An error occurred while retrieving posts. Please try again later."); // Return 500 Internal Server Error
            }
        }


        [HttpGet]
        [Route("post/{id}")]
        public IActionResult GetPost(int id)
        {
            try
            {
                Posts post = _context.Posts.Where(x => x.ID == id).FirstOrDefault();

                if (post == null)
                {
                    return NotFound(); // Return 404 Not Found if no posts are found
                }
                var user = _context.Users.FirstOrDefault(u => u.Id == post.users_id);
                var posts = new List<object>
                {
                    post,
                    user
                };

                return Ok(posts); // Return 200 OK with the list of posts
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                // logger.LogError(ex, "An error occurred while retrieving posts.");

                return StatusCode(500, "An error occurred while retrieving posts. Please try again later."); // Return 500 Internal Server Error
            }
        }


        [HttpPut]
        [Route("post/{id}")]
        public IActionResult UpdatePost(int id, Posts post)
        {
            try
            {
                // Check if the post with the given ID exists in the database
                var existingPost = _context.Posts.FirstOrDefault(p => p.ID == id);
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
        [HttpGet]
        [Route("post/category/{category}")]
        public IActionResult GetPostCategory(string category)
        {
            try
            {
                var post = _context.Posts.Where(p => p.Category.Equals(category)).ToList();
                return StatusCode(200, post);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete]
        [Route("post/{id}")]
        public IActionResult deletePost(int id)
        {
            try
            {

                Posts post = _context.Posts.Where(x => x.ID == id).FirstOrDefault();
                if (post != null)
                {
                    _context.Posts.Remove(post);
                    _context.SaveChanges();
                    return StatusCode(200, "Post deleted successfully");

                }
                else
                {
                    return StatusCode(400, "No post found");
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, "An error occurred while updating the post. Please try again later.");
            }


        }


    }
}
