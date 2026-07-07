using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVVM.Pages.Likes;

[Authorize]
public class ToggleModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public ToggleModel(
        ApplicationDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public int PostId { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var like = await _context.Likes.FirstOrDefaultAsync(l =>
            l.PostId == PostId &&
            l.UserId == user.Id);

        if (like == null)
        {
            _context.Likes.Add(new Like
            {
                PostId = PostId,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            _context.Likes.Remove(like);
        }

        await _context.SaveChangesAsync();

        return RedirectToPage("/Home/Index");
    }
}