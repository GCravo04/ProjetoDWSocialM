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

    public IList<Post> Posts { get; set; } = new List<Post>();

    public AppUser? CurrentUser { get; set; }

    [BindProperty]
    public Post NewPost { get; set; } = new();
    
    [BindProperty]
    public int PostId { get; set; }

    [BindProperty]
    public string CommentContent { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        CurrentUser = await _userManager.GetUserAsync(User);

        Posts = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
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
            return Challenge(); // obriga ao login

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
}