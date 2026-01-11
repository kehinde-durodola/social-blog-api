namespace SocialBlogApi.DTOs.Posts;

public class CreatePostRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public IFormFile? ImageFile { get; set; }
}
