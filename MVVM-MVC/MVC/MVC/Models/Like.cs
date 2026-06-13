namespace MVC.Models;

public class Like
{
    public string UserId { get; set; }

    public int PostId { get; set; }

    public DateTime CreatedAt { get; set; }

    public AppUser User { get; set; }

    public Post Post { get; set; }
}