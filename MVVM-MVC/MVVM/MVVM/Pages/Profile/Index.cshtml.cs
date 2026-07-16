using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVC.Models;

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

    public string ProfileUserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public int PostCount { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public bool IsOwnProfile { get; set; }
    public bool IsFollowing { get; set; }
    public List<PostViewModel> Posts { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            id = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(id))
                return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();

        ProfileUserId = user.Id;
        UserName = user.UserName ?? "Utilizador";
        ProfileImageUrl = user.ProfileImageUrl;

        var meId = _userManager.GetUserId(User);
        IsOwnProfile = meId == user.Id;

        FollowersCount = await _context.Follows.CountAsync(f => f.FollowedUserId == id);
        FollowingCount = await _context.Follows.CountAsync(f => f.FollowerUserId == id);
        IsFollowing = meId != null &&
            await _context.Follows.AnyAsync(f => f.FollowerUserId == meId && f.FollowedUserId == id);

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

    public async Task<IActionResult> OnPostFollowAsync(string id)
    {
        var me = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(me))
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        if (me != id)
        {
            bool jaSegue = await _context.Follows
                .AnyAsync(f => f.FollowerUserId == me && f.FollowedUserId == id);
            if (!jaSegue)
            {
                _context.Follows.Add(new Follow
                {
                    FollowerUserId = me,
                    FollowedUserId = id,
                    CreatedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }
        }
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUnfollowAsync(string id)
    {
        var me = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(me))
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        var follow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerUserId == me && f.FollowedUserId == id);
        if (follow != null)
        {
            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage(new { id });
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