using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVC.Models;

namespace MVVM.Pages.Comments;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public CreateModel(
        ApplicationDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public int PostId { get; set; }

    [BindProperty]
    public string CommentContent { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        if (string.IsNullOrWhiteSpace(CommentContent))
            return RedirectToPage("/Home/Index");

        var comment = new Comment
        {
            Content = CommentContent,
            CreatedAt = DateTime.UtcNow,
            UserId = user.Id,
            PostId = PostId
        };

        _context.Comments.Add(comment);

        await _context.SaveChangesAsync();

        return RedirectToPage("/Home/Index");
    }
}