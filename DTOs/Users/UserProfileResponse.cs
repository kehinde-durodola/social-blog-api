namespace SocialBlogApi.DTOs.Users;

using System.ComponentModel;

public class UserProfileResponse
{
    [Description("Unique user identifier")]
    public int UserId { get; set; }

    [Description("User's email address")]
    public string Email { get; set; } = string.Empty;

    [Description("User's first name")]
    public string FirstName { get; set; } = string.Empty;

    [Description("User's last name")]
    public string LastName { get; set; } = string.Empty;

    [Description("Timestamp when the user created their account (UTC)")]
    public DateTime CreatedAt { get; set; }

    [Description("Total number of posts created by this user")]
    public int PostCount { get; set; }
}
