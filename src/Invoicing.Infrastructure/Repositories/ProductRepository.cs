namespace Invoicing.Infrastructure.Repositories;

using Invoicing.Application.Interfaces;
using Invoicing.Domain.Entities;
using Invoicing.Infrastructure.Persistence;

public class ProductRepository : Repository<Product>, IProductRepository
{
  public ProductRepository(AppDbContext context) : base(context) { }
}