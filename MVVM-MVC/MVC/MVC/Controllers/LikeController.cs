using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Models.DTOs;

namespace MVC.Controllers;

// API dos gostos. Não há endpoints separados para dar e retirar like:
// como a chave é composta (UserId + PostId), um só Toggle resolve os dois casos
// e evita duplicados.
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LikeController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public LikeController(
        ApplicationDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Like>>> GetLikes()
    {
        return await _context.Likes.ToListAsync();
    }

    // O utilizador vem do token/sessão e não do DTO: caso contrário era possível
    // dar likes em nome de outra pessoa.
    [HttpPost("Toggle")]
    public async Task<IActionResult> ToggleLike(LikeDTO dto)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        var like = await _context.Likes.FirstOrDefaultAsync(l =>
            l.PostId == dto.PostId &&
            l.UserId == user.Id);

        bool liked;

        if (like == null)
        {
            _context.Likes.Add(new Like
            {
                UserId = user.Id,
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