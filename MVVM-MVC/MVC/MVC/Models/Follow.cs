namespace MVC.Models;

public class Follow
{
    public string FollowerId { get; set; }

    public string FollowingId { get; set; }

    public DateTime CreatedAt { get; set; }

    public AppUser Follower { get; set; }

    public AppUser Following { get; set; }
}