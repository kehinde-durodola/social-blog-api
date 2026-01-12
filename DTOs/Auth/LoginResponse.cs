namespace SocialBlogApi.DTOs.Auth;

using System.ComponentModel;

public class LoginResponse
{
    [Description("Authenticated user's unique identifier")]
    public int UserId { get; set; }

    [Description("Authenticated user's email address")]
    public string Email { get; set; } = string.Empty;

    [Description("JWT access token (valid for 15 minutes) - use in Authorization header as Bearer token")]
    public string AccessToken { get; set; } = string.Empty;

    [Description("Refresh token (valid for 7 days) - use to obtain a new access token without re-authenticating")]
    public string RefreshToken { get; set; } = string.Empty;

    [Description("Access token expiration time in seconds")]
    public int ExpiresIn { get; set; }
}
