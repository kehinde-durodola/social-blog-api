namespace SocialBlogApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using SocialBlogApi.Core.Exceptions;
using SocialBlogApi.DTOs.Users;
using SocialBlogApi.Services;

/// <summary>
/// User profiles controller for retrieving user information and their posts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Retrieve a user's profile information by user ID.
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User profile details (excludes sensitive data like password, role, ban status)</returns>
    /// <response code="200">User profile found</response>
    /// <response code="404">User not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserProfileResponse>> GetUserProfile(int id)
    {
        try
        {
            var response = await _userService.GetUserProfileAsync(id);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieve all posts created by a specific user with pagination.
    /// </summary>
    /// <param name="id">User ID whose posts to retrieve</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10)</param>
    /// <returns>Paginated list of user's posts</returns>
    /// <response code="200">Posts retrieved successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
    /// <response code="404">User not found</response>
    [HttpGet("{id}/posts")]
    public async Task<IActionResult> GetUserPosts(int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var response = await _userService.GetUserPostsAsync(id, pageNumber, pageSize);
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
