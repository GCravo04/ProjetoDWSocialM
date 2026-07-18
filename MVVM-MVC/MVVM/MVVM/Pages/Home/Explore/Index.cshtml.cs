using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MVVM.Pages.Home.Explore;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<AppUser> Users { get; set; } = new List<AppUser>();

    [BindProperty(SupportsGet = true)]
    public string Search { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(Search))
        {
            query = query.Where(u =>
                u.UserName != null &&
                u.UserName.Contains(Search));
        }

        // Limitado a 50 para uma pesquisa vazia não arrastar a tabela toda
        Users = await query
            .OrderBy(u => u.UserName)
            .Take(50)
            .ToListAsync();
    }
}