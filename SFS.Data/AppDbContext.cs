using global::SFS.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace SFS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Title)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .Property(p => p.Title)
            .HasMaxLength(40);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Product);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Buyer)
            .WithMany(u => u.Orders);

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "User 1" },
            new User { Id = 2, Name = "User 2" },
            new User { Id = 3, Name = "User 3" },
            new User { Id = 4, Name = "User 4" },
            new User { Id = 5, Name = "User 5" },
            new User { Id = 6, Name = "User 6" },
            new User { Id = 7, Name = "User 7" });
    }
}
