namespace SocialBlogApi.Core.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SocialBlogApi.Models;

public class JwtTokenService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;

    public JwtTokenService(IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");
        _secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        _issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
        _audience = jwtSection["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
        _accessTokenExpirationMinutes = int.Parse(jwtSection["AccessTokenExpirationMinutes"] ?? "15");
        _refreshTokenExpirationDays = int.Parse(jwtSection["RefreshTokenExpirationDays"] ?? "7");
    }

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        return Convert.ToBase64String(randomNumber);
    }

    public int ExtractUserId(ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null && int.TryParse(claim.Value, out var userId) ? userId : 0;
    }

    public string ExtractUserRole(ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.Role);
        return claim?.Value ?? string.Empty;
    }

    public string ExtractUserEmail(ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.Email);
        return claim?.Value ?? string.Empty;
    }
}
