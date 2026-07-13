using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Models.DTOs;

namespace MVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FollowController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Follow
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Follow>>> GetFollows()
        {
            return await _context.Follows.ToListAsync();
        }

        // POST: api/Follow/Toggle
        [HttpPost("Toggle")]
        public async Task<IActionResult> ToggleFollow(FollowDTO dto)
        {
            var follow = await _context.Follows.FirstOrDefaultAsync(f =>
                f.FollowerUserId == dto.FollowerUserId &&
                f.FollowedUserId == dto.FollowedUserId);

            bool following;

            if (follow == null)
            {
                _context.Follows.Add(new Follow
                {
                    FollowerUserId = dto.FollowerUserId,
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
}