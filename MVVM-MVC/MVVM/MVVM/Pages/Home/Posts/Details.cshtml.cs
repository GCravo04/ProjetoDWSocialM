using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVVM.Pages.Home.Posts;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public DetailsModel(
        ApplicationDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IList<Post> Posts { get; set; } = new List<Post>();

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
}