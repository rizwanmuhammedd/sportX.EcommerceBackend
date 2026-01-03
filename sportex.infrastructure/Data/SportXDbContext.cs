using Microsoft.EntityFrameworkCore;
using Sportex.Domain.Entities;

namespace Sportex.Infrastructure.Data;

public class SportexDbContext : DbContext
{
    public SportexDbContext(DbContextOptions<SportexDbContext> options)
        : base(options)
    {
    }

    // Tables
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PRODUCT
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(p => p.Price)
                  .HasPrecision(18, 2);   // Money safe

            entity.Property(p => p.StockQuantity)
                  .IsRequired();

            entity.Property(p => p.ImageUrl)
                  .HasMaxLength(500);
        });

        // USER
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                  .HasMaxLength(100);

            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(u => u.PasswordHash)
                  .IsRequired();

            entity.Property(u => u.RefreshToken)
                  .HasMaxLength(500);
        });

        // ORDER
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.TotalAmount)
                  .HasPrecision(18, 2);   // Money safe

            entity.Property(o => o.ShippingAddress)
                  .HasMaxLength(300);
        });

        // ORDER ITEM
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);

            entity.Property(oi => oi.UnitPrice)
                  .HasPrecision(18, 2);  // Money safe
        });


        // CART
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(c => c.Id);
        });

        // WISHLIST
        modelBuilder.Entity<WishlistItem>(entity =>
        {
            entity.HasKey(w => w.Id);
        });
    }
}
