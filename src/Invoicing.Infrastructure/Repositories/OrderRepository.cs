namespace Invoicing.Infrastructure.Repositories;

using Invoicing.Application.Interfaces;
using Invoicing.Domain.Entities;
using Invoicing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class OrderRepository : Repository<Order>, IOrderRepository
{
  public OrderRepository(AppDbContext context) : base(context) { }

  public async Task<Order?> GetByIdWithDetailsAsync(int id)
  {
    return await _context.Orders
        .Include(o => o.Customer)
        .Include(o => o.Items)
            .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(o => o.Id == id);
  }

  public async Task<IEnumerable<Order>> GetAllWithDetailsAsync()
  {
    return await _context.Orders
        .Include(o => o.Customer)
        .Include(o => o.Items)
            .ThenInclude(i => i.Product)
        .ToListAsync();
  }
}