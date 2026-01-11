namespace SocialBlogApi.Core.Services;

using BC = BCrypt.Net.BCrypt;

public class PasswordHashingService
{
    public string HashPassword(string password)
    {
        return BC.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BC.Verify(password, hash);
    }
}
