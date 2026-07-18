using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Models.DTOs;

namespace MVC.Controllers;

// API das publicações. Os métodos de escrita recebem DTOs em vez da entidade Post:
// o cliente só envia o conteúdo, e o autor e as datas são definidos no servidor.
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PostController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public PostController(
        ApplicationDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
    {
        return await _context.Posts.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Post>> GetPost(int id)
    {
        var post = await _context.Posts.FindAsync(id);

        if (post == null)
            return NotFound();

        return post;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPost(int id, PostDto dto)
    {
        var post = await _context.Posts.FindAsync(id);

        if (post == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        if (post.UserId != user.Id && !User.IsInRole("Admin"))
            return Forbid();

        post.Content = dto.Content;
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Post>> PostPost(PostDto dto)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        var post = new Post
        {
            UserId = user.Id,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPost),
            new { id = post.PostId },
            post);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var post = await _context.Posts.FindAsync(id);

        if (post == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        // Só o autor ou um administrador podem apagar a publicação
        if (post.UserId != user.Id && !User.IsInRole("Admin"))
            return Forbid();

        _context.Posts.Remove(post);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}