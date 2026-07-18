using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MVVM.Pages.Home.Comments;

[Authorize]
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

    [BindProperty]
    public int CommentId { get; set; }

    [BindProperty]
    public string Content { get; set; } = string.Empty;

    public int PostId { get; set; }

    // Carrega o comentário para o formulário, se o utilizador o puder editar
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.CommentId == id);

        if (comment == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        // Só o autor ou um admin podem editar
        if (comment.UserId != user!.Id && !User.IsInRole("Admin"))
            return Forbid();

        CommentId = comment.CommentId;
        Content = comment.Content;
        PostId = comment.PostId;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.CommentId == CommentId);

        if (comment == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        if (comment.UserId != user!.Id && !User.IsInRole("Admin"))
            return Forbid();

        comment.Content = Content;

        await _context.SaveChangesAsync();

        return RedirectToPage("/Home/Posts/Details",
            new { id = comment.PostId });
    }
}