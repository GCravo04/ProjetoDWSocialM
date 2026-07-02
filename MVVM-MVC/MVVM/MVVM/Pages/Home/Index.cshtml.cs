using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVVM.Pages.Home;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public IndexModel(
        ApplicationDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    
    public string CurrentFeed { get; set; } = "all";
    public IList<Post> Posts { get; set; } = new List<Post>();

    public AppUser? CurrentUser { get; set; }

    [BindProperty]
    public Post NewPost { get; set; } = new();

    [BindProperty]
    public int PostId { get; set; }

    [BindProperty]
    public string CommentContent { get; set; } = string.Empty;
    
    public async Task OnGetAsync(string feed = "all")
    {
        CurrentFeed = feed;
        CurrentUser = await _userManager.GetUserAsync(User);

        if (feed == "following" && CurrentUser != null)
        {
            var followingIds = await _context.Follows
                .Where(f => f.FollowerUserId == CurrentUser.Id)
                .Select(f => f.FollowedUserId)
                .ToListAsync();

            Posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .Where(p => followingIds.Contains(p.UserId))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
        else
        {
            Posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        NewPost.UserId = user.Id;
        NewPost.CreatedAt = DateTime.UtcNow;
        NewPost.UpdatedAt = null;

        _context.Posts.Add(NewPost);

        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCommentAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        if (string.IsNullOrWhiteSpace(CommentContent))
            return RedirectToPage();

        var comment = new Comment
        {
            Content = CommentContent,
            CreatedAt = DateTime.UtcNow,
            UserId = user.Id,
            PostId = PostId
        };

        _context.Comments.Add(comment);

        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostLikeAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var existingLike = await _context.Likes
            .FirstOrDefaultAsync(l =>
                l.PostId == PostId &&
                l.UserId == user.Id);

        if (existingLike != null)
        {
            _context.Likes.Remove(existingLike);
        }
        else
        {
            _context.Likes.Add(new Like
            {
                PostId = PostId,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToPage();
    }
}