using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Data;
using NetworkingPlatform.Models;

namespace NetworkingPlatform.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ReplyController: ControllerBase
    {
        private readonly AppDbContext _context;
        public ReplyController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("reply/{commentId}")]
        public async Task<IActionResult> GetReplies(int commentId)
        {
            try
            {
                var comment =await _context.PostComments.FirstOrDefaultAsync(p => p.ID == commentId);
                if (comment == null)
                {
                    return NotFound();
                }
                var reply =await _context.Reply.Where(c => c.comment_id == commentId).ToArrayAsync();

                return Ok(reply);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        [Route("reply/{commentId}")]
        public async Task<IActionResult> PostReply(int commentId, [FromBody] Reply c)
        {
            try
            {
                var comment =await _context.PostComments.FirstOrDefaultAsync(p => p.ID == commentId);
                if (comment == null)
                {
                    return NotFound();
                }
                _context.Reply.Add(c);
                _context.SaveChanges();
                return Ok(c);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPatch]
        [Route("reply/{id}")]
        public async Task<IActionResult> EditComment(int id, [FromBody] string content)
        {
            try
            {
                var reply = await _context.Reply.FirstOrDefaultAsync(c => c.ID == id);
                if (reply == null)
                {
                    return NotFound();
                }
                reply.Content = content;
                _context.SaveChanges();
                return Ok(reply);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpDelete]
        [Route("reply/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var reply =await _context.Reply.FirstOrDefaultAsync(_ => _.ID == id);
                if (reply == null)
                {
                    return NotFound();
                }
                _context.Reply.Remove(reply);
                _context.SaveChanges();
                return Ok(reply);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
