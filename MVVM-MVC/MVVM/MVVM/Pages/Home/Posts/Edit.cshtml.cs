using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVVM.Pages.Home.Posts;

public class EditModel : PageModel
{
    [BindProperty]
    public string Content { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        return RedirectToPage("/Home/Index");
    }
}