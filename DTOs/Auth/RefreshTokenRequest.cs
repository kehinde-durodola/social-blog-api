namespace SocialBlogApi.DTOs.Auth;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh token is required")]
    [Description("Valid refresh token from a previous login response (valid for 7 days)")]
    public string RefreshToken { get; set; } = string.Empty;
}
