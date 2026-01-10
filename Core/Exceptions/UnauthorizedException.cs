namespace SocialBlogApi.Core.Exceptions;

public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(string message = "Unauthorized access") : base(message)
    {
    }
}
