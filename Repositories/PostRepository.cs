namespace SocialBlogApi.Repositories;

using Microsoft.EntityFrameworkCore;
using SocialBlogApi.Data;
using SocialBlogApi.Models;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;

    public PostRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _context.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<List<Post>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _context.Posts
            .Include(p => p.User)
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Post>> GetByUserIdAsync(int userId, int pageNumber, int pageSize)
    {
        return await _context.Posts
            .Include(p => p.User)
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Posts.CountAsync(p => !p.IsDeleted);
    }

    public async Task<int> GetCountByUserAsync(int userId)
    {
        return await _context.Posts.CountAsync(p => p.UserId == userId && !p.IsDeleted);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Posts.AnyAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task AddAsync(Post post)
    {
        await _context.Posts.AddAsync(post);
    }

    public async Task UpdateAsync(Post post)
    {
        _context.Posts.Update(post);
    }

    public async Task SoftDeleteAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post != null)
        {
            post.IsDeleted = true;
            _context.Posts.Update(post);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
