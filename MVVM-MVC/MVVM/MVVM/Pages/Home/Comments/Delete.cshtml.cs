using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVVM.Pages.Comments;

[Authorize]
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

    [BindProperty]
    public int CommentId { get; set; }

    [Authorize]
    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var comment = await _context.Comments
            .Include(c => c.Post)
            .FirstOrDefaultAsync(c => c.CommentId == CommentId);

        if (comment == null)
            return NotFound();
        
        // Só o autor do comentário ou um admin o podem apagar
        if (comment.UserId != user.Id && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Home/Posts/Details", new { id = comment.PostId });
    }
}