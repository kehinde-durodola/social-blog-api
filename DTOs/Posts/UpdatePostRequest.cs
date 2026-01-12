namespace SocialBlogApi.DTOs.Posts;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SocialBlogApi.Core.Validation;

public class UpdatePostRequest
{
    [Required(ErrorMessage = "Title is required")]
    [NotEmpty(ErrorMessage = "Title cannot be empty or whitespace only")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    [Description("Updated post title (3-200 characters)")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Body is required")]
    [NotEmpty(ErrorMessage = "Body cannot be empty or whitespace only")]
    [MinLength(10, ErrorMessage = "Body must be at least 10 characters")]
    [Description("Updated post body content (minimum 10 characters)")]
    public string Body { get; set; } = string.Empty;

    [Description("Optional new post image (JPEG, PNG, or WebP; max 5MB)")]
    public IFormFile? ImageFile { get; set; }
}
