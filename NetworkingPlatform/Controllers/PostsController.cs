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
        public async Task<IActionResult> AddPost(Posts Post) //function that add post
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
        public async Task<IActionResult> GetPosts() //get all the posts from the database
        {
            try
            {
                var posts = _context.Posts.Where(p=> p.isDeleted==false).ToList();
                if (posts == null || !posts.Any())
                {
                    return Ok(posts); // Return 404 Not Found if no posts are found
                }

                var postsWithUsers = new List<object>(); // Create a list to store posts along with user information
                foreach (var post in posts)
                {
                    var user =_context.Users.Find(post.users_id); 
                    var likes =_context.Votes.Where(l=>l.post_id == post.ID).ToArray();
                    var likeCount = _context.Votes.Count(l => l.post_id == post.ID && l.voteType == 1); // Count of likes
                    var unlikeCount = _context.Votes.Count(l => l.post_id == post.ID && l.voteType == 0); // Count of unlikes
                    var commentCount = _context.PostComments.Count(c => c.post_id == post.ID); // Count of comments
                    var popularityScore = (2 * likeCount) + (-1 * unlikeCount) + (1 * commentCount); 

                    var postWithUser = new
                    {
                        Post = post,
                        User = user,
                        Likes = likes,
                        popularity = popularityScore
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
        public async Task<IActionResult> GetPost(int id)
        {
            try
            {
                var post = await _context.Posts.Where(x => x.ID == id).FirstOrDefaultAsync();

                if (post == null)
                {
                    return NotFound(); // Return 404 Not Found if no posts are found
                }
                var user =await _context.Users.FirstOrDefaultAsync(u => u.Id == post.users_id);
                var likes = _context.Votes.Where(l=>l.post_id==post.ID).ToArray();
                var finalpost = new List<object>();
                var posts = new
                {
                    Post = post,
                    User = user,
                    Likes = likes
                };
                finalpost.Add(posts);
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
                var postHistory = new PostHistory
                {
                    post_id = existingPost.ID,
                    Title = existingPost.Title,
                    Image = existingPost.Image,
                    Description = existingPost.Description,
                    Category = existingPost.Category,
                    Date = existingPost.Date,
                    users_id = existingPost.users_id,
                    isDeleted = existingPost.isDeleted
                };

                _context.PostHistory.Add(postHistory);
                // Update only specific properties of the post
                existingPost.Title = post.Title;
                existingPost.Image = post.Image;
                existingPost.Description = post.Description;
                existingPost.Category = post.Category;
                existingPost.Date = DateTime.Now;

                _context.SaveChanges();
                //_context.Entry(existingPost).CurrentValues.SetValues(post);
                //_context.SaveChanges();

                return Ok("Post updated successfully"); // Return 200 OK if the update is successful
            }
           
            catch (Exception ex)
            {
                // Handle  exceptions
                return StatusCode(500, "An error occurred while updating the post. Please try again later.");
            }
        }
        [HttpGet]
        [Route("post/category/{category}")]
        public IActionResult GetPostCategory(string category)
        {
            try
            {
                List<Posts> posts;
                if (category == "Popular")
                {
                    posts = _context.Posts.Where(p=> p.isDeleted==false).OrderByDescending(p => _context.Votes.Count(v => v.post_id == p.ID && v.voteType == 1) -
                                      _context.Votes.Count(v => v.post_id == p.ID && v.voteType == 0) +
                                      _context.PostComments.Count(c => c.post_id == p.ID))
               .ThenByDescending(p => p.Date)
                 .ToList();
                }else if(category == "New")
                {
                    posts = _context.Posts.Where(p=> p.isDeleted==false).OrderByDescending(p => _context.Votes.Count(v => v.post_id == p.ID && v.voteType == 1) -
                                     _context.Votes.Count(v => v.post_id == p.ID && v.voteType == 0) +
                                     _context.PostComments.Count(c => c.post_id == p.ID))
              .ThenByDescending(p => p.Date)
                .ToList();

                }
                else
                {
                    posts = _context.Posts
               .Where(p => p.Category.Equals(category) && p.isDeleted==false)
               .OrderByDescending(p => _context.Votes.Count(v => v.post_id == p.ID && v.voteType == 1) -
                                      _context.Votes.Count(v => v.post_id == p.ID && v.voteType == 0) +
                                      _context.PostComments.Count(c => c.post_id == p.ID))
               .ThenByDescending(p => p.Date)
               .ToList();
                }
                var postsWithUsers = new List<object>(); // Create a list to store posts along with user information
                foreach (var post in posts)
                {
                    var user = _context.Users.Find(post.users_id); // Retrieve user information for the post
                    var likes = _context.Votes.Where(l => l.post_id == post.ID).ToArray();
                    // Create an anonymous object containing post and user information
                    var postWithUser = new
                    {
                        Post = post,
                        User = user,
                        Likes = likes,
                    };

                    postsWithUsers.Add(postWithUser); // Add post with user information to the list
                }
                return StatusCode(200, postsWithUsers);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete]
        [Route("post/{id}")]
        public async Task<IActionResult> deletePost(int id)//delete the post given the post id
        {
            try
            {

                var post = await _context.Posts.Where(x => x.ID == id).FirstOrDefaultAsync();

                if (post != null)
                {
                    post.isDeleted = true;
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

        [HttpDelete]
        [Route("post/admin/{id}")]
        public async Task<IActionResult> deletePostByAdmin(int id)//function for admin to delete thet particular post given by id
        {
            try
            {

                var post = await _context.Posts.Where(x => x.ID == id).FirstOrDefaultAsync();
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

        [HttpGet]
        [Route("posts/only")]
        public async Task<IActionResult> GetPostsOnly()
        {
            try
            {
                var posts = _context.Posts.ToList();
                if (posts == null || !posts.Any())
                {
                    return Ok(posts); // Return 404 Not Found if no posts are found
                }

                return Ok(posts); // Return 200 OK with the list of posts

            }
            catch (Exception ex)
            {

                return StatusCode(500, "An error occurred while retrieving posts. Please try again later."); // Return 500 Internal Server Error
            }
        }

        [HttpGet]
        [Route("posts/admin")]
        public async Task<IActionResult> getUserPosts()
        {
            try
            {
                var posts = await _context.Posts.Select(post => new
                   {
                       post.ID,
                       post.Title,
                       post.Image,
                       post.Description,
                       post.Category,
                       post.Date,
                       post.isDeleted,
                       post.users_id,
                       user = _context.Users.FirstOrDefault(u=> u.Id==post.users_id).UserName,
                       LikeCount = _context.Votes.Count(v => v.post_id == post.ID && v.voteType == 1), // Count of likes
                       UnlikeCount = _context.Votes.Count(v => v.post_id == post.ID && v.voteType == 0), // Count of unlikes
                       commentCount = _context.PostComments.Count(p => p.post_id == post.ID),
                       popularity = (2 * _context.Votes.Count(v => v.post_id == post.ID && v.voteType == 1)) +
                                  (-1 * _context.Votes.Count(v => v.post_id == post.ID && v.voteType == 0)) +
                                  (1 * _context.PostComments.Count(p => p.post_id == post.ID))
                }).OrderByDescending(post => post.popularity)
                   .ToListAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}
