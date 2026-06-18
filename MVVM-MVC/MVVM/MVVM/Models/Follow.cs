using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MVC.Models;
public class Follow
{
    [Key]
    public int FollowId { get; set; }

    [ForeignKey(nameof(FollowerUser))]
    public string? FollowerUserId { get; set; }

    [ForeignKey(nameof(FollowedUser))]
    public string FollowedUserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    [InverseProperty(nameof(AppUser.Following))]
    public AppUser? FollowerUser { get; set; }

    [InverseProperty(nameof(AppUser.Followers))]
    public AppUser FollowedUser { get; set; } = null!;
}