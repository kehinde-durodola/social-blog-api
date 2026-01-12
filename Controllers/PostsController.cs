namespace SocialBlogApi.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialBlogApi.Core.Exceptions;
using SocialBlogApi.Core.Services;
using SocialBlogApi.DTOs.Posts;
using SocialBlogApi.Services;

/// <summary>
/// Blog posts controller for creating, reading, updating, and deleting posts.
/// Supports pagination, image uploads, and role-based access control.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly PostService _postService;
    private readonly JwtTokenService _jwtTokenService;

    public PostsController(PostService postService, JwtTokenService jwtTokenService)
    {
        _postService = postService;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Retrieve all published posts with pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10)</param>
    /// <returns>Paginated list of posts</returns>
    /// <response code="200">Posts retrieved successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
    [HttpGet]
    public async Task<IActionResult> GetAllPosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var response = await _postService.GetAllPostsAsync(pageNumber, pageSize);
            return Ok(response);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieve a specific post by ID.
    /// </summary>
    /// <param name="id">Post ID</param>
    /// <returns>Post details</returns>
    /// <response code="200">Post found</response>
    /// <response code="404">Post not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<PostResponse>> GetPostById(int id)
    {
        try
        {
            var post = await _postService.GetPostByIdAsync(id);
            return Ok(post);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Create a new blog post (authenticated users only).
    /// Supports optional image upload (max 5MB, JPEG/PNG/WebP).
    /// </summary>
    /// <param name="request">Post title, body content, and optional image file</param>
    /// <returns>Created post details</returns>
    /// <response code="201">Post created successfully</response>
    /// <response code="400">Invalid post data or image upload failed</response>
    /// <response code="401">Authentication required</response>
    /// <response code="403">User is banned from creating posts</response>
    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<PostResponse>> CreatePost([FromForm] CreatePostRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = _jwtTokenService.ExtractUserId(User);
            var response = await _postService.CreatePostAsync(request, userId);
            return CreatedAtAction(nameof(GetPostById), new { id = response.PostId }, response);
        }
        catch (NotFoundException)
        {
            return NotFound(new { message = "User not found" });
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing blog post (owner or admin only).
    /// Supports optional image replacement.
    /// </summary>
    /// <param name="id">Post ID to update</param>
    /// <param name="request">Updated title, body, and optional new image</param>
    /// <returns>Updated post details</returns>
    /// <response code="200">Post updated successfully</response>
    /// <response code="400">Invalid post data</response>
    /// <response code="401">Authentication required</response>
    /// <response code="403">Insufficient permissions (not owner or admin)</response>
    /// <response code="404">Post not found</response>
    [HttpPut("{id}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<PostResponse>> UpdatePost(int id, [FromForm] UpdatePostRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = _jwtTokenService.ExtractUserId(User);
            var userRole = _jwtTokenService.ExtractUserRole(User);
            var response = await _postService.UpdatePostAsync(id, request, userId, userRole);
            return Ok(response);
        }
        catch (NotFoundException)
        {
            return NotFound(new { message = "Post not found" });
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a blog post (owner or admin only).
    /// Uses soft delete - post remains in database but is hidden from users.
    /// </summary>
    /// <param name="id">Post ID to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">Post deleted successfully</response>
    /// <response code="401">Authentication required</response>
    /// <response code="403">Insufficient permissions (not owner or admin)</response>
    /// <response code="404">Post not found</response>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            var userId = _jwtTokenService.ExtractUserId(User);
            var userRole = _jwtTokenService.ExtractUserRole(User);
            await _postService.DeletePostAsync(id, userId, userRole);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound(new { message = "Post not found" });
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
    }

    /// <summary>
    /// Retrieve all posts by a specific user with pagination.
    /// </summary>
    /// <param name="userId">User ID whose posts to retrieve</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10)</param>
    /// <returns>Paginated list of user's posts</returns>
    /// <response code="200">Posts retrieved successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
    /// <response code="404">User not found</response>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPosts(int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var response = await _postService.GetUserPostsAsync(userId, pageNumber, pageSize);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
