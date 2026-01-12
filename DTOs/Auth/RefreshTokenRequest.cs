namespace SocialBlogApi.DTOs.Auth;

using System.ComponentModel.DataAnnotations;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}
