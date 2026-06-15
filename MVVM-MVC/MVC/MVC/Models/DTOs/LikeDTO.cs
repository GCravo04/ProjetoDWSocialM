namespace MVC.Models.DTOs;

public class LikeDTO
{
    public string UserId { get; set; } = string.Empty;

    public int PostId { get; set; }

    public DateTime CreatedAt { get; set; }
}