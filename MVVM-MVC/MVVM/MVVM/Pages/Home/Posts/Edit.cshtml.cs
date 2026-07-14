using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVVM.Pages.Home.Posts;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public EditModel(
        ApplicationDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IList<Post> Posts { get; set; } = new List<Post>();

    [BindProperty]
    public int PostId { get; set; }

    [BindProperty]
    public string Content { get; set; } = string.Empty;

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

        post.Content = Content;

        await _context.SaveChangesAsync();

        return RedirectToPage("/Home/Index");;
    }
}