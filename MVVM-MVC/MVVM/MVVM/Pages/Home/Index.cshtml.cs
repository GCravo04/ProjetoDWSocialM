using MVC.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MVVM.Pages.Home;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Lista de posts que a vista vai mostrar
    public List<PostViewModel> Posts { get; set; } = new();

    public async Task OnGetAsync()
    {
        Posts = await _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostViewModel
            {
                PostId = p.PostId,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                AuthorName = p.User.UserName ?? "Utilizador",
                CommentCount = p.Comments.Count,
                LikeCount = p.Likes.Count
            })
            .ToListAsync();
    }

    // Modelo simples só com o que o feed precisa de mostrar
    public class PostViewModel
    {
        public int PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int CommentCount { get; set; }
        public int LikeCount { get; set; }
    }
}