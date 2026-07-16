using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVVM.Hubs;

// Hub de SignalR do feed.
// Os likes passam por aqui em vez de um POST normal, para que a contagem
// apareça atualizada em todos os browsers abertos sem ninguém dar refresh.
[Authorize]
public class FeedHub : Hub
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public FeedHub(
        ApplicationDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Toggle: se o utilizador já tinha dado like ao post remove-o, caso contrário cria.
    public async Task ToggleLike(int postId)
    {
        var user = await _userManager.GetUserAsync(Context.User);

        if (user == null)
            return;

        var like = await _context.Likes.FirstOrDefaultAsync(l =>
            l.PostId == postId &&
            l.UserId == user.Id);

        bool liked;

        if (like == null)
        {
            _context.Likes.Add(new Like
            {
                PostId = postId,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            });

            liked = true;
        }
        else
        {
            _context.Likes.Remove(like);

            liked = false;
        }

        await _context.SaveChangesAsync();

        var totalLikes = await _context.Likes
            .CountAsync(l => l.PostId == postId);

        // Avisa todos os clientes ligados.
        // o userID vem incluído porque só o browser de quem carregou é que deve trocar o estado do coração
        await Clients.All.SendAsync(
            "LikeUpdated",
            postId,
            totalLikes,
            user.Id,
            liked);
    }
}