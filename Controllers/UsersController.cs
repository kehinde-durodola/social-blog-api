namespace SocialBlogApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using SocialBlogApi.Core.Exceptions;
using SocialBlogApi.DTOs.Users;
using SocialBlogApi.Services;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

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
