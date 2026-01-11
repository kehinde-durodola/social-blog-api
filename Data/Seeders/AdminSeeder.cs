namespace SocialBlogApi.Data.Seeders;

using SocialBlogApi.Core.Services;
using SocialBlogApi.Models;

public class AdminSeeder
{
    private readonly AppDbContext _context;
    private readonly PasswordHashingService _passwordService;
    private readonly IConfiguration _configuration;

    public AdminSeeder(AppDbContext context, PasswordHashingService passwordService, IConfiguration configuration)
    {
        _context = context;
        _passwordService = passwordService;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        if (_context.Users.Any())
            return;

        var adminEmail = _configuration["DefaultAdmin:Email"] ?? throw new InvalidOperationException("DefaultAdmin:Email not configured");
        var adminPassword = _configuration["DefaultAdmin:Password"] ?? throw new InvalidOperationException("DefaultAdmin:Password not configured");
        var firstName = _configuration["DefaultAdmin:FirstName"] ?? throw new InvalidOperationException("DefaultAdmin:FirstName not configured");
        var lastName = _configuration["DefaultAdmin:LastName"] ?? throw new InvalidOperationException("DefaultAdmin:LastName not configured");

        var admin = new User
        {
            Email = adminEmail,
            PasswordHash = _passwordService.HashPassword(adminPassword),
            FirstName = firstName,
            LastName = lastName,
            Role = "Admin",
            IsBanned = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(admin);
        await _context.SaveChangesAsync();
    }
}
