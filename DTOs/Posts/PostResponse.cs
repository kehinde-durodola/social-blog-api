namespace SocialBlogApi.DTOs.Posts;

using System.ComponentModel;

public class PostResponse
{
    [Description("Unique post identifier")]
    public int PostId { get; set; }

    [Description("Post title")]
    public string Title { get; set; } = string.Empty;

    [Description("Post body content")]
    public string Body { get; set; } = string.Empty;

    [Description("URL to the post's image (null if no image)")]
    public string? ImageUrl { get; set; }

    [Description("Full name of the post author")]
    public string AuthorName { get; set; } = string.Empty;

    [Description("Timestamp when the post was created (UTC)")]
    public DateTime CreatedAt { get; set; }

    [Description("Timestamp when the post was last updated (UTC)")]
    public DateTime UpdatedAt { get; set; }

    [Description("Indicates if the post is soft-deleted")]
    public bool IsDeleted { get; set; }
}
