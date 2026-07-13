using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Models.DTOs;

namespace MVC.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FollowController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public FollowController(
        ApplicationDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Follow>>> GetFollows()
    {
        return await _context.Follows.ToListAsync();
    }

    [HttpPost("Toggle")]
    public async Task<IActionResult> ToggleFollow(FollowDTO dto)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        var follow = await _context.Follows.FirstOrDefaultAsync(f =>
            f.FollowerUserId == user.Id &&
            f.FollowedUserId == dto.FollowedUserId);

        bool following;

        if (follow == null)
        {
            _context.Follows.Add(new Follow
            {
                FollowerUserId = user.Id,
                FollowedUserId = dto.FollowedUserId,
                CreatedAt = DateTime.UtcNow
            });

            following = true;
        }
        else
        {
            _context.Follows.Remove(follow);
            following = false;
        }

        await _context.SaveChangesAsync();

        var totalFollowers = await _context.Follows.CountAsync(f =>
            f.FollowedUserId == dto.FollowedUserId);

        return Ok(new
        {
            FollowedUserId = dto.FollowedUserId,
            Following = following,
            TotalFollowers = totalFollowers
        });
    }
}