namespace SocialBlogApi.Services;

using SocialBlogApi.Core.Exceptions;
using SocialBlogApi.DTOs.Common;
using SocialBlogApi.DTOs.Posts;
using SocialBlogApi.DTOs.Users;
using SocialBlogApi.Models;
using SocialBlogApi.Repositories;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;

    public UserService(IUserRepository userRepository, IPostRepository postRepository)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;
    }

    public async Task<UserProfileResponse> GetUserProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        var postCount = await _postRepository.GetCountByUserAsync(userId);

        return new UserProfileResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            PostCount = postCount
        };
    }

    public async Task<PaginatedResponse<PostResponse>> GetUserPostsAsync(int userId, int pageNumber, int pageSize)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        var totalItems = await _postRepository.GetCountByUserAsync(userId);
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var posts = await _postRepository.GetByUserIdAsync(userId, pageNumber, pageSize);
        var items = posts.Select(p => MapToPostResponse(p)).ToList();

        return new PaginatedResponse<PostResponse>
        {
            Items = items,
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            HasNextPage = pageNumber < totalPages,
            HasPreviousPage = pageNumber > 1
        };
    }

    public async Task<bool> IsBannedAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.IsBanned ?? false;
    }

    private PostResponse MapToPostResponse(Post post)
    {
        return new PostResponse
        {
            PostId = post.Id,
            Title = post.Title,
            Body = post.Body,
            ImageUrl = post.ImageUrl,
            AuthorName = $"{post.User.FirstName} {post.User.LastName}",
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            IsDeleted = post.IsDeleted
        };
    }
}
