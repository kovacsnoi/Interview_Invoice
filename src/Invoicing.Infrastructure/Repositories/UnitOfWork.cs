namespace Invoicing.Infrastructure.Repositories;

using Invoicing.Application.Interfaces;
using Invoicing.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
  private readonly AppDbContext _context;

  public UnitOfWork(AppDbContext context)
  {
    _context = context;
    Products = new ProductRepository(_context);
    Customers = new CustomerRepository(_context);
    Orders = new OrderRepository(_context);
  }

  public IProductRepository Products { get; }
  public ICustomerRepository Customers { get; }
  public IOrderRepository Orders { get; }

  public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}