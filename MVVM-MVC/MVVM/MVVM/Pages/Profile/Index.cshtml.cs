using MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MVVM.Pages.Profile;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public string UserName { get; set; } = string.Empty;
    public int PostCount { get; set; }
    public bool IsOwnProfile { get; set; }
    public List<PostViewModel> Posts { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string? id)
    {
        // Sem id -> usa o utilizador autenticado
        if (string.IsNullOrEmpty(id))
        {
            id = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(id))
                return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();

        UserName = user.UserName ?? "Utilizador";
        IsOwnProfile = _userManager.GetUserId(User) == user.Id;

        Posts = await _context.Posts
            .Where(p => p.UserId == id)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostViewModel
            {
                PostId = p.PostId,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                CommentCount = p.Comments.Count,
                LikeCount = p.Likes.Count
            })
            .ToListAsync();

        PostCount = Posts.Count;
        return Page();
    }

    public class PostViewModel
    {
        public int PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int CommentCount { get; set; }
        public int LikeCount { get; set; }
    }
}