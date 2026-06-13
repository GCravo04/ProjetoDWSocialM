namespace MVC.Models;

public class Post
{
    public int PostId { get; set; }

    public string UserId { get; set; }

    public string Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public AppUser User { get; set; }

    public ICollection<Comment> Comments { get; set; }
        = new List<Comment>();

    public ICollection<Like> Likes { get; set; }
        = new List<Like>();
}