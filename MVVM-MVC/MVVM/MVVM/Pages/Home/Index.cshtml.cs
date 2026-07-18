using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVVM.Pages.Home;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public IndexModel(
        ApplicationDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public string CurrentFeed { get; set; } = "all";

    public IList<Post> Posts { get; set; } = new List<Post>();

    public AppUser? CurrentUser { get; set; }

    public async Task OnGetAsync(string feed = "all")
    {
        CurrentFeed = feed;
        CurrentUser = await _userManager.GetUserAsync(User);

        // Include do autor, likes e comentários: o feed mostra isto,
        // por isso vem numa única ida à BD em vez de multiplas queries
        IQueryable<Post> query = _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .ThenInclude(c => c.User);

        // Separador "A Seguir": filtra o feed pelos ids de quem o utilizador segue
        if (feed == "following" && CurrentUser != null)
        {
            var followingIds = await _context.Follows
                .Where(f => f.FollowerUserId == CurrentUser.Id)
                .Select(f => f.FollowedUserId)
                .ToListAsync();

            query = query.Where(p => followingIds.Contains(p.UserId));
        }

        Posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}