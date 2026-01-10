namespace SocialBlogApi.Core.Exceptions;

public class ForbiddenException : ApplicationException
{
    public ForbiddenException(string message = "Forbidden access") : base(message)
    {
    }
}
