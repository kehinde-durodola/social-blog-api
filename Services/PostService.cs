namespace SocialBlogApi.Services;

using SocialBlogApi.Core.Exceptions;
using SocialBlogApi.DTOs.Common;
using SocialBlogApi.DTOs.Posts;
using SocialBlogApi.Models;
using SocialBlogApi.Repositories;

public class PostService
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;

    public PostService(IPostRepository postRepository, IUserRepository userRepository)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
    }

    public async Task<PostResponse> CreatePostAsync(CreatePostRequest request, int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        if (user.IsBanned)
            throw new ForbiddenException("User is banned and cannot create posts");

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ApplicationException("Post title cannot be empty");

        if (string.IsNullOrWhiteSpace(request.Body))
            throw new ApplicationException("Post body cannot be empty");

        var post = new Post
        {
            Title = request.Title,
            Body = request.Body,
            ImageUrl = request.ImageUrl,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _postRepository.AddAsync(post);
        await _postRepository.SaveChangesAsync();

        return MapToPostResponse(post, user);
    }

    public async Task<PaginatedResponse<PostResponse>> GetAllPostsAsync(int pageNumber, int pageSize)
    {
        var totalItems = await _postRepository.GetCountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var posts = await _postRepository.GetAllAsync(pageNumber, pageSize);
        var items = posts.Select(p => MapToPostResponse(p, p.User)).ToList();

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

    public async Task<PostResponse> GetPostByIdAsync(int postId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
            throw new NotFoundException("Post not found");

        return MapToPostResponse(post, post.User);
    }

    public async Task<PostResponse> UpdatePostAsync(int postId, UpdatePostRequest request, int userId, string userRole)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
            throw new NotFoundException("Post not found");

        if (post.UserId != userId && userRole != "Admin")
            throw new ForbiddenException("You cannot edit this post");

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ApplicationException("Post title cannot be empty");

        if (string.IsNullOrWhiteSpace(request.Body))
            throw new ApplicationException("Post body cannot be empty");

        post.Title = request.Title;
        post.Body = request.Body;
        post.ImageUrl = request.ImageUrl;
        post.UpdatedAt = DateTime.UtcNow;

        await _postRepository.UpdateAsync(post);
        await _postRepository.SaveChangesAsync();

        return MapToPostResponse(post, post.User);
    }

    public async Task<bool> DeletePostAsync(int postId, int userId, string userRole)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
            throw new NotFoundException("Post not found");

        if (post.UserId != userId && userRole != "Admin")
            throw new ForbiddenException("You cannot delete this post");

        await _postRepository.SoftDeleteAsync(postId);
        await _postRepository.SaveChangesAsync();

        return true;
    }

    public async Task<PaginatedResponse<PostResponse>> GetUserPostsAsync(int userId, int pageNumber, int pageSize)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        var totalItems = await _postRepository.GetCountByUserAsync(userId);
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var posts = await _postRepository.GetByUserIdAsync(userId, pageNumber, pageSize);
        var items = posts.Select(p => MapToPostResponse(p, p.User)).ToList();

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

    private PostResponse MapToPostResponse(Post post, User author)
    {
        return new PostResponse
        {
            PostId = post.Id,
            Title = post.Title,
            Body = post.Body,
            ImageUrl = post.ImageUrl,
            AuthorName = $"{author.FirstName} {author.LastName}",
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            IsDeleted = post.IsDeleted
        };
    }
}
