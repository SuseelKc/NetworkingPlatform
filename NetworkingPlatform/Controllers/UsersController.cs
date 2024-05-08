using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Data;
using NetworkingPlatform.Models;
using System.Security.Cryptography;
using System.Text;

namespace NetworkingPlatform.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        ///
        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> RegisterUser(Users User)
        {
            try
            {
                // Validate email format
                if (!IsValidEmail(User.Email))
                {
                    return BadRequest("Invalid email format.");
                }
                User.PasswordHash = HashPassword(User.PasswordHash);

                User.NormalizedEmail = User.Email.ToUpper(); 
                User.NormalizedUserName= User.Email.ToUpper();
                _context.Users.Add(User);
                _context.SaveChanges();
                return Ok(User);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<Users>();
            return passwordHasher.HashPassword(null, password);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        ///

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();

                var usersWithPopularity = new List<object>();

                foreach (var user in users)
                {
                    var posts = await _context.Posts.Where(p => p.users_id == user.Id).ToListAsync();

                    int popularityScore = 0;

                    foreach (var post in posts)
                    {
                        // Calculate popularity score for each post and add to total popularity score
                        popularityScore += (2*(_context.Votes.Count(v => v.post_id == post.ID && v.voteType == 1))) // Likes
                                            + (-1*(_context.Votes.Count(v => v.post_id == post.ID && v.voteType == 0))) // Unlikes
                                            + (1* (_context.PostComments.Count(c => c.post_id == post.ID))); // Comments
                    }

                    var userWithPopularity = new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        email = user.Email,
                        role = user.UserRole,
                        posts = _context.Posts.Count(c=> c.users_id==user.Id),
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

        [HttpGet]
        [Route("user/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
               var user = await _context.Users.FirstAsync(u=>u.Id==id);
                return Ok(user);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("user/posts/{id}")]
        public async Task<IActionResult> getUserPosts(string id)
        {
            try
            {
                var posts = await _context.Posts
                   .Where(p => p.users_id == id)
                   .Select(post => new
                   {
                       post.ID,
                       post.Title,
                       post.Image,
                       post.Description,
                       post.Category,
                       post.Date,
                       post.users_id,
                       LikeCount = _context.Votes.Count(v => v.post_id == post.ID && v.voteType == 1), // Count of likes
                       UnlikeCount = _context.Votes.Count(v => v.post_id == post.ID && v.voteType == 0), // Count of unlikes
                       commentCount = _context.PostComments.Count(p=> p.post_id== post.ID)
                   })
                   .ToListAsync();
                return Ok(posts);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // for log in 



        //






















        //[HttpPost]
        //[Route("AddUser")]
        //public string AddUser(Users User) {
        //    _context.Users.Add(User);
        //    _context.SaveChanges();
        //    return "User Added Sucessfully!";
        //}

        //[HttpDelete]
        //[Route("DeleteUser")]
        //public string DeleteUser(int id)
        //{

        //    Users User = _context.Users.Where(x => x.ID == id).FirstOrDefault();
        //    if (User != null)
        //    {
        //        _context.Users.Remove(User);
        //        _context.SaveChanges();
        //        return "User Deleted";

        //    }
        //    else
        //    {
        //        return "No User Found";
        //    }


        //}
    }
}
