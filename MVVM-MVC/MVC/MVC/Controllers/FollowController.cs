using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        // GET: api/Follow/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Follow>> GetFollow(int id)
        {
            var follow = await _context.Follows.FindAsync(id);

            if (follow == null)
            {
                return NotFound();
            }

            return follow;
        }

        // PUT: api/Follow/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFollow(int id, FollowDTO dto)
        {
            var follow = await _context.Follows.FindAsync(id);

            if (follow == null)
                return NotFound();

            follow.FollowerUserId = dto.FollowerUserId;
            follow.FollowedUserId = dto.FollowedUserId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Follow
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Follow>> PostFollow(FollowDTO dto)
        {
            var follow = new Follow
            {
                FollowerUserId = dto.FollowerUserId,
                FollowedUserId = dto.FollowedUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFollow),
                new { id = follow.FollowId },
                follow);
        }

        // DELETE: api/Follow/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFollow(int id)
        {
            var follow = await _context.Follows.FindAsync(id);
            if (follow == null)
            {
                return NotFound();
            }

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FollowExists(int id)
        {
            return _context.Follows.Any(e => e.FollowId == id);
        }
    }
}
