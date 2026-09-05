namespace Invoicing.Infrastructure.Repositories;

using Invoicing.Application.Interfaces;
using Invoicing.Domain.Entities;
using Invoicing.Infrastructure.Persistence;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
  public CustomerRepository(AppDbContext context) : base(context) { }
}