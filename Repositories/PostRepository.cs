using api_bui_xuan_thang.Models;
using api_bui_xuan_thang.Repositories;
using Microsoft.EntityFrameworkCore;

public class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _context;

    public PostRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _context.Posts
            .Include(post => post.User) // Bao gồm thông tin User
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByUserIdAsync(string userId)
    {
        // Lọc bài đăng của người dùng theo userId
        return await _context.Posts
            .Include(p => p.User)
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _context.Posts.FindAsync(id);
    }

    public async Task<Post> CreateAsync(Post post, string userId)
    {
        post.UserId = userId;
        post.DateCreate = DateTime.UtcNow;

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return post;
    }

    public async Task UpdateAsync(Post post, string userId)
    {
        var existingPost = await _context.Posts.FindAsync(post.Id);

        if (existingPost == null)
        {
            throw new KeyNotFoundException("Post not found");
        }

        if (existingPost.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this post.");
        }

        existingPost.Title = post.Title;
        existingPost.Description = post.Description;
        existingPost.Image = post.Image;

        _context.Posts.Update(existingPost);
        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(int id, string userId)
    {
        var post = await _context.Posts.FindAsync(id);

        if (post == null)
        {
            throw new KeyNotFoundException("Post not found");
        }

        if (post.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this post.");
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
    }

}

