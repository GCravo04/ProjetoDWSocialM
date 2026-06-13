using Microsoft.AspNetCore.Identity;

namespace MVC.Models;

public class AppUser: IdentityUser
{
    public string? ProfileImageUrl { get; set; }
}