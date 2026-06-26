using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVC.Models;

namespace MVVM.Pages.Home.Posts;

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
    public Post NewPost { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Redirect("/Identity/Account/Login");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "É necessário iniciar sessão para publicar.");

                return Redirect("/Identity/Account/Login");
            }

            NewPost.UserId = user.Id;
            NewPost.CreatedAt = DateTime.UtcNow;

            _context.Posts.Add(NewPost);

            await _context.SaveChangesAsync();

            return RedirectToPage("/Home/Index");
        }
        catch (Exception e)
        {
            ModelState.AddModelError(
                string.Empty,
                $"Erro ao publicar: {e.Message}");

            return Page();
        }
    }
}