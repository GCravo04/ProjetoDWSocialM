namespace MVC.Models.DTOs;

public class FollowDTO
{
    public int FollowId { get; set; }

    public string? FollowerUserId { get; set; }

    public string FollowedUserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}