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
                User.NormalizedUserName = User.Email.ToUpper();
                _context.Users.Add(User);
                _context.SaveChanges();
                return Ok(User);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                        posts = _context.Posts.Count(c => c.users_id == user.Id),
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
                var user = await _context.Users.FirstAsync(u => u.Id == id);
                return Ok(user);
            }
            catch (Exception ex)
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
                   .Where(p => p.users_id == id && p.isDeleted == false)
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
                       commentCount = _context.PostComments.Count(p => p.post_id == post.ID)
                   })
                   .ToListAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPatch]
        [Route("user/{id}")]
        public async Task<IActionResult> UpdateUser(string id, string type, string content)
        {
            try
            {
                var user = await _context.Users.FirstAsync(u => u.Id == id);
                if (user == null)
                {
                    return NotFound("No user found");
                }
                if (type == "image")
                {
                    user.Image = content;
                }
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("user/admin/{id}")]
        public async Task<IActionResult> DeleteUserByAdmin(string id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return NotFound("No user found");
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok("User deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        //changepassword
        [HttpPost]
        [Route("user/changepassword/{id}")]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePassword model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return NotFound("No user found");
                }

                // Assuming ChangePasswordModel contains properties OldPassword and NewPassword
                if (!VerifyPassword(user, model.OldPassword))
                {
                    return BadRequest("Incorrect old password");
                }

                // Change password logic
                user.PasswordHash = HashPassword(model.NewPassword);

                await _context.SaveChangesAsync();

                return Ok("User password changed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        private bool VerifyPassword(Users user, string password)
        {
            // You need to implement your own logic for verifying passwords,
            // for example, comparing hash of provided password with stored hash.
            // This might involve using a library like ASP.NET Core Identity.
            // For demonstration purposes, let's assume password verification logic here.
            var passwordHasher = new PasswordHasher<Users>();
            var result = passwordHasher.VerifyHashedPassword(null, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success;
        }

        [HttpPatch]
        [Route("changeprofile/{id}")]
        public async Task<IActionResult> ChangeProfile(string id, [FromBody] string img)
        {
            try { 
                var user = await _context.Users.FirstOrDefaultAsync(u=> u.Id== id);
                if(user == null)
                {
                    return NotFound();
                }
                user.Image = img;
                await _context.SaveChangesAsync();
                return Ok("Profile updated");

            } catch(Exception ex)
            {
                return StatusCode(400, ex.Message);
            }

        }


    }
}
