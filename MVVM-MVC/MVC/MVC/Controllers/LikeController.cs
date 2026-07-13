using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Models.DTOs;

namespace MVC.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LikeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LikeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Like
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Like>>> GetLikes()
    {
        return await _context.Likes.ToListAsync();
    }

    // POST: api/Like/Toggle
    [HttpPost("Toggle")]
    public async Task<IActionResult> ToggleLike(LikeDTO dto)
    {
        var like = await _context.Likes.FirstOrDefaultAsync(l =>
            l.PostId == dto.PostId &&
            l.UserId == dto.UserId);

        bool liked;

        if (like == null)
        {
            _context.Likes.Add(new Like
            {
                UserId = dto.UserId,
                PostId = dto.PostId,
                CreatedAt = DateTime.UtcNow
            });

            liked = true;
        }
        else
        {
            _context.Likes.Remove(like);

            liked = false;
        }

        await _context.SaveChangesAsync();

        var totalLikes = await _context.Likes
            .CountAsync(l => l.PostId == dto.PostId);

        return Ok(new
        {
            PostId = dto.PostId,
            TotalLikes = totalLikes,
            Liked = liked
        });
    }
}