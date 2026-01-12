namespace SocialBlogApi.Core.Validation;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Validates that a string is not null, empty, or whitespace-only.
/// </summary>
public class NotEmptyAttribute : ValidationAttribute
{
    public NotEmptyAttribute()
    {
        ErrorMessage = "The {0} field cannot be empty or contain only whitespace.";
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
            return false;

        if (value is string stringValue)
            return !string.IsNullOrWhiteSpace(stringValue);

        return true;
    }
}
