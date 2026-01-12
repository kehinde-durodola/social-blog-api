namespace SocialBlogApi.DTOs.Posts;

using System.ComponentModel.DataAnnotations;
using SocialBlogApi.Core.Validation;

public class CreatePostRequest
{
    [Required(ErrorMessage = "Title is required")]
    [NotEmpty(ErrorMessage = "Title cannot be empty or whitespace only")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Body is required")]
    [NotEmpty(ErrorMessage = "Body cannot be empty or whitespace only")]
    [MinLength(10, ErrorMessage = "Body must be at least 10 characters")]
    public string Body { get; set; } = string.Empty;

    public IFormFile? ImageFile { get; set; }
}
