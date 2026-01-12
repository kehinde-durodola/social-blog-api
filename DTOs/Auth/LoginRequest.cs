namespace SocialBlogApi.DTOs.Auth;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    [Description("User's registered email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    [Description("User's password")]
    public string Password { get; set; } = string.Empty;
}
