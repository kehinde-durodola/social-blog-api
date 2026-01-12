namespace SocialBlogApi.DTOs.Common;

using System.ComponentModel;

public class PaginatedResponse<T>
{
    [Description("List of items for the current page")]
    public List<T> Items { get; set; } = new();

    [Description("Total number of items across all pages")]
    public int TotalItems { get; set; }

    [Description("Total number of pages available")]
    public int TotalPages { get; set; }

    [Description("Current page number (1-based)")]
    public int CurrentPage { get; set; }

    [Description("Number of items per page")]
    public int PageSize { get; set; }

    [Description("Whether a next page exists")]
    public bool HasNextPage { get; set; }

    [Description("Whether a previous page exists")]
    public bool HasPreviousPage { get; set; }
}
