using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Data;
using NetworkingPlatform.Models;

namespace NetworkingPlatform.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CommentController: ControllerBase
    {
        private readonly AppDbContext _context;
        public CommentController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("comment/{postId}")]
        public async Task<IActionResult> GetComments(int postId)
        {
            try
            {
                var post =await _context.Posts.FirstOrDefaultAsync(p=> p.ID == postId);
                if(post == null)
                {
                    return NotFound();
                }
                var comment = _context.PostComments.Where(c => c.post_id == postId).Select(com =>
                new
                {
                    id = com.ID,
                    content = com.Content,
                    date = com.Date,
                    author = _context.Users.FirstOrDefault(u => u.Id == com.users_id).UserName,
                    avatar = _context.Users.FirstOrDefault(u=> u.Id==com.users_id).Image,
                    users_id = com.users_id
                });
              
               
                return Ok(comment);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        [Route("comment/{postId}")]
        public async Task<IActionResult> PostComment(int postId,[FromBody] PostComments c)
        {
            try
            {
                var post = await _context.Posts.FirstOrDefaultAsync(p=>p.ID == postId);

                if (post == null)
                {
                    return NotFound();
                }
                _context.PostComments.Add(c);
                _context.SaveChanges();
                return Ok(c);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPatch]
        [Route("comment/{id}")]
        public async Task<IActionResult> EditComment(int id, [FromBody] string content)
        {
            try
            {
                var comment = _context.PostComments.FirstOrDefault(c=>c.ID == id);
                if (comment == null)
                {
                    return NotFound();
                }
                var commentHistory = new CommentHistory

                {
                    Content = comment.Content,
                    users_id = comment.users_id,
                    comment_id = comment.ID,
                    Date = comment.Date,
                   
                };

                _context.CommentHistory.Add(commentHistory);
                comment.Content = content;
                comment.Date = DateTime.Now;
                _context.SaveChanges();
                return Ok(comment);

            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpDelete]
        [Route("comment/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var comment = _context.PostComments.FirstOrDefault(_ => _.ID == id);
                if(comment == null)
                {
                    return NotFound();
                }
                _context.PostComments.Remove(comment);
                _context.SaveChanges();
                return Ok(comment);

            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
