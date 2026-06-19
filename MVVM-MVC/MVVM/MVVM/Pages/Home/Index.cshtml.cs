using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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

    public async Task OnGetAsync()
    {
        CurrentUser = await _userManager.GetUserAsync(User);

        Posts = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
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
}