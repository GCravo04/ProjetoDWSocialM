using System.ComponentModel.DataAnnotations;

namespace MVC.Models;

public class Post
{
    [Key]
    public int PostId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public AppUser User { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; }
        = new List<Comment>();

    public ICollection<Like> Likes { get; set; }
        = new List<Like>();
}