using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Models.DTOs;

namespace MVC.Controllers;

// API dos comentários. Tal como nos posts, recebe DTOs e não a entidade Comment,
// para o cliente não conseguir escolher o autor nem a data.
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public CommentController(
        ApplicationDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: api/Comment/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Comment>> GetComment(int id)
    {
        var comment = await _context.Comments.FindAsync(id);

        if (comment == null)
            return NotFound();

        return comment;
    }

    // PUT: api/Comment/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutComment(int id, CommentDTO dto)
    {
        var comment = await _context.Comments.FindAsync(id);

        if (comment == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        // Apenas o dono ou Admin
        if (comment.UserId != user.Id && !User.IsInRole("Admin"))
            return Forbid();

        comment.Content = dto.Content;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Comment
    [HttpPost]
    public async Task<ActionResult<Comment>> PostComment(CommentDTO dto)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        var comment = new Comment
        {
            UserId = user.Id,
            PostId = dto.PostId,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return Ok(comment);
    }

    // DELETE: api/Comment/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _context.Comments.FindAsync(id);

        if (comment == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        // Apenas o dono ou Admin
        if (comment.UserId != user.Id && !User.IsInRole("Admin"))
            return Forbid();

        _context.Comments.Remove(comment);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}