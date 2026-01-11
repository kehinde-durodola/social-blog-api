namespace SocialBlogApi.Repositories;

using SocialBlogApi.Models;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(int id);
    Task<List<Post>> GetAllAsync(int pageNumber, int pageSize);
    Task<List<Post>> GetByUserIdAsync(int userId, int pageNumber, int pageSize);
    Task<int> GetCountAsync();
    Task<int> GetCountByUserAsync(int userId);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Post post);
    Task UpdateAsync(Post post);
    Task SoftDeleteAsync(int id);
    Task SaveChangesAsync();
}
