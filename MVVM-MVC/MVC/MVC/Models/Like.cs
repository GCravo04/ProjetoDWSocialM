

namespace MVC.Models;
public class Like
{
    public string UserId { get; set; } = string.Empty;

    public int PostId { get; set; }

    public DateTime CreatedAt { get; set; }

    public AppUser User { get; set; } = null!;

    public Post Post { get; set; } = null!;
}