using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using MVC.Models;

namespace MVVM.Pages.Profile;

[Authorize]
public class EditModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public EditModel(UserManager<AppUser> userManager,
                     SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "O nome de utilizador é obrigatório.")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "O nome deve ter entre 3 e 50 caracteres.")]
        [Display(Name = "Nome de utilizador")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Password atual")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Display(Name = "Nova password")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "A nova password deve ter pelo menos 6 caracteres.")]
        public string? NewPassword { get; set; }

        [Display(Name = "Confirmar nova password")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "As passwords não coincidem.")]
        public string? ConfirmPassword { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        Input = new InputModel
        {
            UserName = user.UserName ?? string.Empty,
            Email    = user.Email    ?? string.Empty,
        };

        return Page();
    }
    
    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!string.IsNullOrWhiteSpace(Input.NewPassword) &&
            string.IsNullOrWhiteSpace(Input.CurrentPassword))
        {
            ModelState.AddModelError(nameof(Input.CurrentPassword),
                "Indica a password atual para a poderes alterar.");
        }

        if (!ModelState.IsValid) return Page();

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var errors = new List<string>();

        if (user.UserName != Input.UserName)
        {
            var result = await _userManager.SetUserNameAsync(user, Input.UserName);
            if (!result.Succeeded)
                errors.AddRange(result.Errors.Select(e => e.Description));
        }

        if (user.Email != Input.Email)
        {
            var result = await _userManager.SetEmailAsync(user, Input.Email);
            if (!result.Succeeded)
                errors.AddRange(result.Errors.Select(e => e.Description));
        }

        if (!string.IsNullOrWhiteSpace(Input.NewPassword) &&
            !string.IsNullOrWhiteSpace(Input.CurrentPassword))
        {
            var result = await _userManager.ChangePasswordAsync(
                user, Input.CurrentPassword, Input.NewPassword);
            if (!result.Succeeded)
                errors.AddRange(result.Errors.Select(e => e.Description));
        }

        if (errors.Any())
        {
            foreach (var e in errors)
                ModelState.AddModelError(string.Empty, e);
            return Page();
        }

        await _signInManager.RefreshSignInAsync(user);

        TempData["SuccessMessage"] = "Perfil atualizado com sucesso!";
        return RedirectToPage();
    }
}
