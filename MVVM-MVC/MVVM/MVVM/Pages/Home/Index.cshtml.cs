using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MVVM.Pages.Home;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Post> Posts { get; set; }
        = new List<Post>();

    [BindProperty]
    public Post NewPost { get; set; }
        = new();

    public async Task OnGetAsync()
    {
        Posts = await _context.Posts
            .Include(p => p.User)
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

        NewPost.CreatedAt = DateTime.UtcNow;

        _context.Posts.Add(NewPost);

        await _context.SaveChangesAsync();

        return RedirectToPage();
    }
}