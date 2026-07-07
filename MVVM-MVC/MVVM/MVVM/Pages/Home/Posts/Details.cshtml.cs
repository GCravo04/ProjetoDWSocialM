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

   
    public Post Post { get; set; } = null!;
    public AppUser? CurrentUser { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        CurrentUser = await _userManager.GetUserAsync(User);
        if (id == null)
        {
            return NotFound();
        }

        var post = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.PostId == id);

        if (post == null)
        {
            return NotFound();
        }

        Post = post;
        return Page();
    }
}