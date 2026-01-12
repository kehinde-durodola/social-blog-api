namespace SocialBlogApi.DTOs.Auth;

using System.ComponentModel;

public class RefreshTokenResponse
{
    [Description("New JWT access token (valid for 15 minutes)")]
    public string AccessToken { get; set; } = string.Empty;

    [Description("Access token expiration time in seconds")]
    public int ExpiresIn { get; set; }
}
