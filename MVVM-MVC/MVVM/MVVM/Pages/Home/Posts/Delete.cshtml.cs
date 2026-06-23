using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MVVM.Pages.Home.Posts;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public DeleteModel(
        ApplicationDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IList<Post> Posts { get; set; } = new List<Post>();

    [BindProperty]
    public int PostId { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Redirect("/Identity/Account/Login");
        }

        Posts = await _context.Posts
            .Where(p => p.UserId == user.Id)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Redirect("/Identity/Account/Login");
        }

        var post = await _context.Posts
            .FirstOrDefaultAsync(p =>
                p.PostId == PostId &&
                p.UserId == user.Id);

        if (post == null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Publicação não encontrada.");

            return await OnGetAsync();
        }

        _context.Posts.Remove(post);

        await _context.SaveChangesAsync();

        return RedirectToPage();
    }
}