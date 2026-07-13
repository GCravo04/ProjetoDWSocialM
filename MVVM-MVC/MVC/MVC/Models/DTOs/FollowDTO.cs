using System.ComponentModel.DataAnnotations;

namespace MVC.Models.DTOs;

public class FollowDTO
{
    [Required]
    public string FollowerUserId { get; set; } = string.Empty;

    [Required]
    public string FollowedUserId { get; set; } = string.Empty;
}