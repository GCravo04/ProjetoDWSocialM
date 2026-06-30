using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVVM.Pages.Profile;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public DeleteModel(
        ApplicationDbContext context,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

  
    public IActionResult OnGet()
    {
        return Page();
    }

  
    public async Task<IActionResult> OnPostAsync(string deletePassword)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

   
        var passwordOk = await _userManager.CheckPasswordAsync(user, deletePassword);
        if (!passwordOk)
        {
            TempData["DeleteError"] = "Password incorreta. A conta não foi eliminada.";
            return RedirectToPage();
        }
        
        var comments = _context.Comments
            .Where(c => c.UserId == user.Id);

        _context.Comments.RemoveRange(comments);

        var likes = _context.Likes
            .Where(l => l.UserId == user.Id);

        _context.Likes.RemoveRange(likes);

        var posts = _context.Posts
            .Where(p => p.UserId == user.Id);

        _context.Posts.RemoveRange(posts);

        await _context.SaveChangesAsync();

       
        await _signInManager.SignOutAsync();

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            TempData["DeleteError"] = "Ocorreu um erro ao eliminar a conta. Tenta novamente.";
            return RedirectToPage();
        }
        
        return RedirectToPage("/Index");
    }
}