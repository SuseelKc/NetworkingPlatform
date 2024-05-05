using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetComments(int postId)
        {
            try
            {
                var post = _context.Posts.FirstOrDefault(p=> p.ID == postId);
                if(post == null)
                {
                    return NotFound();
                }
                var comment = _context.PostComments.Where(c => c.post_id == postId).ToArray();
               
                return Ok(comment);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        [Route("comment/{postId}")]
        public IActionResult PostComment(int postId,[FromBody] PostComments c)
        {
            try
            {
                var post = _context.Posts.FirstOrDefault(p => p.ID == postId);
                if(post == null)
                {
                    return NotFound();
                }
                _context.PostComments.Add(c);
                _context.SaveChanges();
                return Ok(c);

            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPatch]
        [Route("comment/{id}")]
        public IActionResult EditComment(int id, [FromBody] string content)
        {
            try
            {
                var comment = _context.PostComments.FirstOrDefault(c=>c.ID == id);
                if(comment == null)
                {
                    return NotFound();
                }
                comment.Content = content;
                _context.SaveChanges();
                return Ok(comment);

            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpDelete]
        [Route("comment/{id}")]
        public IActionResult DeleteComment(int id)
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
