namespace MVC.Models.DTOs;

public class APPUserDTO
{
    public string Id { get; set; } = string.Empty;

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? ProfileImageUrl { get; set; }
}