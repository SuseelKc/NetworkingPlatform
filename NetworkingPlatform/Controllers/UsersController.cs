using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Data;
using NetworkingPlatform.Models;

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

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users.Select(u=> new
                {
                    id=u.Id,
                    userName= u.UserName,
                    email= u.Email,
                    posts=_context.Posts.Where(p=>p.users_id==u.Id).Count(),
                }).ToListAsync();

                return Ok(users);
            }catch (Exception ex)
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
