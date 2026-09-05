namespace Invoicing.Application.Interfaces;

public interface IUnitOfWork
{
  IProductRepository Products { get; }
  ICustomerRepository Customers { get; }
  IOrderRepository Orders { get; }

  Task<int> SaveChangesAsync();
}