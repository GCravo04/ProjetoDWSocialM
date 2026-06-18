using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using MVC.Models;

namespace MVC.Models;
public class AppUser : IdentityUser
{
    public string? ProfileImageUrl { get; set; }

    // Posts criados pelo utilizador
    public ICollection<Post> Posts { get; set; } = new List<Post>();

    // Comentários criados pelo utilizador
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    // Likes do utilizador
    public ICollection<Like> Likes { get; set; } = new List<Like>();

    [InverseProperty(nameof(Follow.FollowerUser))]
    public ICollection<Follow> Following { get; set; } = new List<Follow>();

    [InverseProperty(nameof(Follow.FollowedUser))]
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();
}