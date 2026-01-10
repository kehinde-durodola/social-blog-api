namespace SocialBlogApi.Core.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string message = "Resource not found") : base(message)
    {
    }
}
