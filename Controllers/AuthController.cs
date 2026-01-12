namespace SocialBlogApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using SocialBlogApi.Core.Exceptions;
using SocialBlogApi.DTOs.Auth;
using SocialBlogApi.Services;

/// <summary>
/// Authentication controller for user registration, login, and token refresh.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user account.
    /// </summary>
    /// <param name="request">User registration details including email, password, first name, and last name</param>
    /// <returns>Newly created user information</returns>
    /// <response code="201">User successfully registered</response>
    /// <response code="400">Invalid input data or email already exists</response>
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var response = await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), response);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Authenticate user and obtain access and refresh tokens.
    /// </summary>
    /// <param name="request">User email and password</param>
    /// <returns>Access token, refresh token, and user information</returns>
    /// <response code="200">Authentication successful</response>
    /// <response code="400">Invalid email or password format</response>
    /// <response code="401">Invalid credentials or user not found</response>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var response = await _authService.LoginAsync(request.Email, request.Password);
            return Ok(response);
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Refresh an expired access token using a valid refresh token.
    /// </summary>
    /// <param name="request">Valid refresh token</param>
    /// <returns>New access token</returns>
    /// <response code="200">Token successfully refreshed</response>
    /// <response code="401">Invalid or expired refresh token</response>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(response);
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
