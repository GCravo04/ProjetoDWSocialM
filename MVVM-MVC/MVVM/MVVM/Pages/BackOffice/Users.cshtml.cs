using MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MVVM.Pages.BackOffice;

[Authorize(Roles = "Admin")]   // <- só administradores acedem
public class UsersModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;

    public UsersModel(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public List<UserRow> Users { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string Search { get; set; } = string.Empty;

    public class UserRow
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }

    public async Task OnGetAsync()
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(Search))
            query = query.Where(u => u.UserName != null && u.UserName.Contains(Search));

        var users = await query.OrderBy(u => u.UserName).Take(100).ToListAsync();

        Users = new List<UserRow>();
        foreach (var u in users)
        {
            Users.Add(new UserRow
            {
                Id = u.Id,
                UserName = u.UserName ?? "(sem nome)",
                IsAdmin = await _userManager.IsInRoleAsync(u, "Admin")
            });
        }
    }

    public async Task<IActionResult> OnPostToggleAdminAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            await _userManager.RemoveFromRoleAsync(user, "Admin");
        else
            await _userManager.AddToRoleAsync(user, "Admin");

        return RedirectToPage(new { Search });
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // Impede um admin de se apagar a si próprio
        var meId = _userManager.GetUserId(User);
        if (user.Id != meId)
            await _userManager.DeleteAsync(user);

        return RedirectToPage(new { Search });
    }
}