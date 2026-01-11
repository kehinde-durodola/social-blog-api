namespace SocialBlogApi.DTOs.Posts;

public class UpdatePostRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}
