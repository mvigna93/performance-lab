using Microsoft.EntityFrameworkCore;
using PerformanceLab.Api.Entities;

namespace PerformanceLab.Api.Data;

public sealed class PerformanceLabDbContext(DbContextOptions<PerformanceLabDbContext> options)
    : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.FirstName).HasMaxLength(80);
            entity.Property(x => x.LastName).HasMaxLength(80);
            entity.Property(x => x.Email).HasMaxLength(200);
            entity.Property(x => x.City).HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Sku).HasMaxLength(40);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.Category).HasMaxLength(80);
            entity.Property(x => x.Price).HasPrecision(12, 2);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(x => x.OrderNumber).HasMaxLength(40);
            entity.Property(x => x.ShippingAddress).HasMaxLength(300);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
            entity.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(x => x.UnitPrice).HasPrecision(12, 2);
            entity.Property(x => x.DiscountPercent).HasPrecision(5, 2);
        });
    }
}
