namespace MVC.Models;

using System.ComponentModel.DataAnnotations;

public class Comment
{
    [Key]
    public int CommentId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int PostId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public AppUser User { get; set; } = null!;

    public Post Post { get; set; } = null!;
}