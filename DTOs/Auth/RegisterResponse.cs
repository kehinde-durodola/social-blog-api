namespace SocialBlogApi.DTOs.Auth;

using System.ComponentModel;

public class RegisterResponse
{
    [Description("Newly created user's unique identifier")]
    public int UserId { get; set; }

    [Description("Newly created user's email address")]
    public string Email { get; set; } = string.Empty;

    [Description("Newly created user's first name")]
    public string FirstName { get; set; } = string.Empty;

    [Description("Newly created user's last name")]
    public string LastName { get; set; } = string.Empty;

    [Description("Success message confirming registration")]
    public string Message { get; set; } = string.Empty;
}
