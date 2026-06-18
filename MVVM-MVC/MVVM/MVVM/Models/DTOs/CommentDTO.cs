namespace MVC.Models.DTOs;

public class CommentDTO
{
    public int CommentId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int PostId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}