namespace Invoicing.Application.Interfaces;

using Invoicing.Domain.Entities;

public interface IOrderRepository : IRepository<Order>
{
  // Egy rendelést a tételeivel (Items) és a hozzá tartozó termékekkel együtt kérünk le,
  // mert a számla generáláshoz mindkettőre szükség van egyszerre.
  Task<Order?> GetByIdWithDetailsAsync(int id);

  Task<IEnumerable<Order>> GetAllWithDetailsAsync();
}