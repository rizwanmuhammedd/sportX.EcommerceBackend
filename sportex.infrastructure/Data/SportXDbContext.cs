





using Microsoft.EntityFrameworkCore;
using Sportex.Domain.Entities;
using Sportex.Infrastructure.Data; // For SportexDbContext

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
    public DbSet<ShippingAddress> ShippingAddresses { get; set; }

    // NEW: Review entities
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ReviewHelpful> ReviewHelpfuls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);

        // ---------------- PRODUCT ----------------
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
            entity.Property(p => p.Price).HasPrecision(18, 2);
            entity.Property(p => p.StockQuantity).IsRequired();
            entity.Property(p => p.ImageUrl).HasMaxLength(500);
            entity.Property(p => p.Category).HasConversion<string>();
            entity.Property(p => p.IsActive).HasDefaultValue(true);
        });

        // ---------------- USER ----------------
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.RefreshToken).HasMaxLength(500);
            entity.Property(u => u.Role).HasMaxLength(20);
            entity.Property(u => u.Otp).HasMaxLength(10);
            entity.Property(u => u.OtpExpiry);
        });

        // ---------------- ORDER ----------------
        // ---------------- ORDER ----------------
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.TotalAmount)
                  .HasPrecision(18, 2);

            entity.Property(o => o.ShippingSnapshot)
                  .HasMaxLength(400);

            entity.Property(o => o.Status)
                  .HasConversion<string>();

            entity.HasOne(o => o.User)
                  .WithMany()
                  .HasForeignKey(o => o.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(o => o.ShippingAddress)
      .WithMany()
      .HasForeignKey(o => o.ShippingAddressId)
      .OnDelete(DeleteBehavior.Restrict)
      .IsRequired(false);



        });


        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
        });

        // ---------------- REVIEW ----------------
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.ProductId, e.UserId }).IsUnique();

            entity.Property(e => e.Comment)
                  .HasMaxLength(1000)
                  .IsRequired();   // ✅ CHANGE THIS

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);


     

        });


        // ---------------- REVIEW HELPFUL ----------------
        // ---------------- REVIEW HELPFUL ----------------
        modelBuilder.Entity<ReviewHelpful>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ReviewId, e.UserId }).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Review)
                .WithMany()
                .HasForeignKey(e => e.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Changed from Cascade to NoAction
        });

        // ---------------- CART & WISHLIST ----------------
        modelBuilder.Entity<CartItem>(entity => entity.HasKey(c => c.Id));

        modelBuilder.Entity<WishlistItem>()
            .HasIndex(x => new { x.UserId, x.ProductId })
            .IsUnique();
    }
}