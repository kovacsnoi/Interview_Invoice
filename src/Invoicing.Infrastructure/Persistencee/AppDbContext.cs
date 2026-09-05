namespace Invoicing.Infrastructure.Persistence;

using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  public DbSet<Product> Products => Set<Product>();
  public DbSet<Customer> Customers => Set<Customer>();
  public DbSet<Order> Orders => Set<Order>();
  public DbSet<OrderItem> OrderItems => Set<OrderItem>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Termék ár precizitása
    modelBuilder.Entity<Product>()
        .Property(p => p.UnitPrice)
        .HasPrecision(18, 2);

    // Relációk és integritásvédelmi szabályok
    modelBuilder.Entity<Order>()
        .HasOne(o => o.Customer)
        .WithMany(c => c.Orders)
        .HasForeignKey(o => o.CustomerId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<OrderItem>()
        .HasOne(oi => oi.Order)
        .WithMany(o => o.Items)
        .HasForeignKey(oi => oi.OrderId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<OrderItem>()
        .HasOne(oi => oi.Product)
        .WithMany(p => p.OrderItems)
        .HasForeignKey(oi => oi.ProductId)
        .OnDelete(DeleteBehavior.Restrict);

    // --- MINTAADATOK (Seed) a minta számla azonnali teszteléséhez ---
    modelBuilder.Entity<Customer>().HasData(
        new Customer { Id = 1, Name = "ABC Ltd", Country = "Hungary", Address = "Budapest, Fő u. 1." }
    );

    modelBuilder.Entity<Product>().HasData(
        new Product { Id = 1, Name = "Laptop", Category = "Electronics", UnitPrice = 1000m, IsHazardous = false, IsFragile = true, IsDiscountEligible = false },
        new Product { Id = 2, Name = "Mouse", Category = "Electronics", UnitPrice = 20m, IsHazardous = false, IsFragile = false, IsDiscountEligible = true },
        new Product { Id = 3, Name = "Battery Pack", Category = "Supplies", UnitPrice = 50m, IsHazardous = true, IsFragile = false, IsDiscountEligible = false }
    );

    // A feladatleírásban látható mintaszámla rendelése (2 Laptop, 3 Mouse):
    modelBuilder.Entity<Order>().HasData(
        new Order { Id = 1, CustomerId = 1, OrderDate = new DateTime(2025, 3, 15, 0, 0, 0, DateTimeKind.Utc) }
    );

    modelBuilder.Entity<OrderItem>().HasData(
        new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2 },
        new OrderItem { Id = 2, OrderId = 1, ProductId = 2, Quantity = 3 }
    );
  }
}