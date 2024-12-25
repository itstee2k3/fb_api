using Microsoft.EntityFrameworkCore;

namespace api.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Product> Products { get; set; }     
    
    public DbSet<Cart> Carts { get; set; }

    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Chỉ định khóa chính kết hợp (UserId + ProductId)
        modelBuilder.Entity<Cart>().HasKey(c => new { c.UserId, c.ProductId });
        
        base.OnModelCreating(modelBuilder);
    }
}
