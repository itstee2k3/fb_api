using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api_bui_xuan_thang.Models;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Product> Products { get; set; }     
    
    public DbSet<Cart> Carts { get; set; }

    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Chỉ định khóa chính kết hợp (UserId + ProductId)
        builder.Entity<Cart>().HasKey(c => new { c.UserId, c.ProductId });
        
        base.OnModelCreating(builder);
        builder.Entity<User>().Property(u => u.Initials).HasMaxLength(5);
        builder.HasDefaultSchema("identity");
    }
}
