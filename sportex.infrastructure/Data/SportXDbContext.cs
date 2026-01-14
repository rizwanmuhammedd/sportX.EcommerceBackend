using Microsoft.EntityFrameworkCore;
using Sportex.Domain.Entities;

namespace Sportex.Infrastructure.Data;

public class SportexDbContext : DbContext
{
    public SportexDbContext(DbContextOptions<SportexDbContext> options)
        : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Payment>()
    .Property(x => x.Amount)
    .HasPrecision(18, 2);   // ₹999,999,999,999,999.99 safe


        // ---------------- PRODUCT ----------------
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
            entity.Property(p => p.Price).HasPrecision(18, 2);
            entity.Property(p => p.StockQuantity).IsRequired();
            entity.Property(p => p.ImageUrl).HasMaxLength(500);
            entity.Property(p => p.Category).HasConversion<string>();

            entity.Property(p => p.IsActive)          // 🔥 FIXED
                  .HasDefaultValue(true);
        });

        // ---------------- USER ----------------
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.RefreshToken).HasMaxLength(500);
            entity.Property(u => u.Role).HasMaxLength(20);   // Admin / User

            entity.Property(u => u.Otp)              // 🔥 FIXED
                  .HasMaxLength(10);

            entity.Property(u => u.OtpExpiry);       // 🔥 FIXED
        });

        // ---------------- ORDER ----------------
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.TotalAmount).HasPrecision(18, 2);
            entity.Property(o => o.ShippingAddress).HasMaxLength(300);
            entity.Property(o => o.Status).HasConversion<string>();
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<CartItem>(entity => entity.HasKey(c => c.Id));
        modelBuilder.Entity<WishlistItem>()
            .HasIndex(x => new { x.UserId, x.ProductId })
            .IsUnique();
    }

}
