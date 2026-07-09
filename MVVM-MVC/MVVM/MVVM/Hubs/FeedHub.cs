using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVVM.Hubs;

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

        await Clients.All.SendAsync(
            "LikeUpdated",
            postId,
            totalLikes,
            user.Id,
            liked);
    }
}