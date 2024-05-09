using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Data;

namespace NetworkingPlatform.Controllers
{
    [Route("api/")]
    [ApiController]
    public class DashboardController: ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("dashboard/info")]
        public async Task<IActionResult> getDashboardInfo()  /// get the dashboard details such as counts and other metrics defiled below for admin
        {
            try
            {
                var count = new List<object>();
              
                    var user = await _context.Users.CountAsync();
                    var posts = await _context.Posts.CountAsync();
                    var likes = await _context.Votes.CountAsync(v=> v.voteType==1);
                    var unlikes = await _context.Votes.CountAsync(v=> v.voteType==0);
                    var comments = await _context.PostComments.CountAsync();
                var reply = await _context.Reply.CountAsync();
                    var counts = new
                    {
                        Users = user,
                        Posts = posts,
                        UpVotes= likes,
                        DownVotes = unlikes,
                        Comments = comments,
                        Replies = reply 
                    };
                count.Add(counts);

                return Ok(count);



            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("dashboard/posts/")]
        public async Task<IActionResult> GetPosts(int year, int month)
        {
            try
            {
                var posts = await _context.Posts.Where(p=> !p.isDeleted && p.Date.Value.Year == year && p.Date.Value.Month == month).Select(post => new
                {
                    post.ID,
                    post.Title,
                    post.Image,
                    post.Description,
                    post.Category,
                    post.Date,
                    post.isDeleted,
                    post.users_id,
                    user = _context.Users.FirstOrDefault(u => u.Id == post.users_id).UserName,
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
                return StatusCode(500, "An error occurred while retrieving posts. Please try again later."); // Return 500 Internal Server Error
            }
        }

        [HttpGet]
        [Route("dashboard/users/")]
        public async Task<IActionResult> GetUsers(int year, int month)
        {
            try
            {
                var users = await _context.Users.ToListAsync();

                var usersWithPopularity = new List<object>();

                foreach (var user in users)
                {
                    var posts = await _context.Posts
                        .Where(p => p.users_id == user.Id && p.Date.HasValue && p.Date.Value.Year == year && p.Date.Value.Month == month)
                        .ToListAsync();

                    int popularityScore = 0;

                    foreach (var post in posts)
                    {
                        // Calculate popularity score for each post and add to total popularity score
                        popularityScore += (2 * (_context.Votes.Count(v => v.post_id == post.ID && v.voteType == 1))) // Likes
                                            + (-1 * (_context.Votes.Count(v => v.post_id == post.ID && v.voteType == 0))) // Unlikes
                                            + (1 * (_context.PostComments.Count(c => c.post_id == post.ID))); // Comments
                    }

                    var userWithPopularity = new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        email = user.Email,
                        role = user.UserRole,
                        image = user.Image,
                        posts = posts.Count,
                        popularity = popularityScore
                    };

                    usersWithPopularity.Add(userWithPopularity);
                }

                // Order users by popularity score

                return Ok(usersWithPopularity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}
