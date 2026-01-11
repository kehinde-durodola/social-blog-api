namespace SocialBlogApi.Services;

using BC = BCrypt.Net.BCrypt;
using SocialBlogApi.Core.Exceptions;
using SocialBlogApi.Core.Services;
using SocialBlogApi.DTOs.Auth;
using SocialBlogApi.Models;
using SocialBlogApi.Repositories;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordHashingService _passwordHashingService;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        PasswordHashingService passwordHashingService,
        JwtTokenService jwtTokenService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.ExistsAsync(request.Email))
            throw new ApplicationException("Email already registered");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = _passwordHashingService.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = "User",
            IsBanned = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Message = "Registration successful"
        };
    }

    public async Task<LoginResponse> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new UnauthorizedException("Invalid email or password");

        if (!_passwordHashingService.VerifyPassword(password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password");

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshTokenExpirationDays = _configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays");
        
        user.RefreshTokenHash = BC.HashPassword(refreshToken);
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        var accessTokenExpirationMinutes = _configuration.GetValue<int>("Jwt:AccessTokenExpirationMinutes");
        
        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = accessTokenExpirationMinutes
        };
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => 
            u.RefreshTokenHash != null && 
            BC.Verify(refreshToken, u.RefreshTokenHash) &&
            u.RefreshTokenExpiresAt > DateTime.UtcNow);

        if (user == null)
            throw new UnauthorizedException("Invalid or expired refresh token");

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var accessTokenExpirationMinutes = _configuration.GetValue<int>("Jwt:AccessTokenExpirationMinutes");

        return new RefreshTokenResponse
        {
            AccessToken = accessToken,
            ExpiresIn = accessTokenExpirationMinutes
        };
    }
}
