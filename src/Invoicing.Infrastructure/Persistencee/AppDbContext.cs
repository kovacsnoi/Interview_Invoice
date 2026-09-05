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
        new Customer { Id = 1, Name = "ABC Ltd", Country = "Hungary", Address = "Budapest, Fő u. 1." },
        new Customer { Id = 2, Name = "Tech Solutions Kft", Country = "Hungary", Address = "Debrecen, Piac u. 12." },
        new Customer { Id = 3, Name = "Nordic Imports AS", Country = "Norway", Address = "Oslo, Karl Johans gate 5." }
    );

    modelBuilder.Entity<Product>().HasData(
        new Product { Id = 1, Name = "Laptop", Category = "Electronics", UnitPrice = 1000m, IsHazardous = false, IsFragile = true, IsDiscountEligible = false },
        new Product { Id = 2, Name = "Mouse", Category = "Electronics", UnitPrice = 20m, IsHazardous = false, IsFragile = false, IsDiscountEligible = true },
        new Product { Id = 3, Name = "Battery Pack", Category = "Supplies", UnitPrice = 50m, IsHazardous = true, IsFragile = false, IsDiscountEligible = false },
        new Product { Id = 4, Name = "Monitor", Category = "Electronics", UnitPrice = 300m, IsHazardous = false, IsFragile = true, IsDiscountEligible = false },
        new Product { Id = 5, Name = "Keyboard", Category = "Electronics", UnitPrice = 45m, IsHazardous = false, IsFragile = false, IsDiscountEligible = true }
    );

    modelBuilder.Entity<Order>().HasData(
        new Order { Id = 1, CustomerId = 1, OrderDate = new DateTime(2025, 3, 15, 0, 0, 0, DateTimeKind.Utc) },
        new Order { Id = 2, CustomerId = 1, OrderDate = new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc) },
        new Order { Id = 3, CustomerId = 2, OrderDate = new DateTime(2025, 5, 10, 0, 0, 0, DateTimeKind.Utc) },
        new Order { Id = 4, CustomerId = 3, OrderDate = new DateTime(2025, 6, 20, 0, 0, 0, DateTimeKind.Utc) }
    );

    modelBuilder.Entity<OrderItem>().HasData(
        // Order 1 (ABC Ltd) — a feladatleírás mintaszámlája: 2 Laptop, 3 Mouse
        new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2 },
        new OrderItem { Id = 2, OrderId = 1, ProductId = 2, Quantity = 3 },

        // Order 2 (ABC Ltd) — tartalmaz veszélyes terméket (Battery Pack)
        new OrderItem { Id = 3, OrderId = 2, ProductId = 3, Quantity = 1 },

        // Order 3 (Tech Solutions Kft) — nagyobb rendelés, több tétellel
        new OrderItem { Id = 4, OrderId = 3, ProductId = 1, Quantity = 5 },
        new OrderItem { Id = 5, OrderId = 3, ProductId = 4, Quantity = 3 },
        new OrderItem { Id = 6, OrderId = 3, ProductId = 5, Quantity = 4 },

        // Order 4 (Nordic Imports AS) — vegyesen kedvezményes, törékeny és veszélyes tétel
        new OrderItem { Id = 7, OrderId = 4, ProductId = 2, Quantity = 10 },
        new OrderItem { Id = 8, OrderId = 4, ProductId = 3, Quantity = 2 },
        new OrderItem { Id = 9, OrderId = 4, ProductId = 4, Quantity = 1 }
    );
  }
}