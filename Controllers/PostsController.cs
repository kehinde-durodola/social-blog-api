namespace SocialBlogApi.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialBlogApi.Core.Exceptions;
using SocialBlogApi.Core.Services;
using SocialBlogApi.DTOs.Posts;
using SocialBlogApi.Services;

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

    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<PostResponse>> CreatePost([FromForm] CreatePostRequest request)
    {
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

    [HttpPut("{id}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<PostResponse>> UpdatePost(int id, [FromForm] UpdatePostRequest request)
    {
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
