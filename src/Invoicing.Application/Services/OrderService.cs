namespace Invoicing.Application.Services;

using Invoicing.Application.Interfaces;
using Invoicing.Application.Models;
using Invoicing.Domain.Entities;
using Invoicing.Application.Mapping;

public class OrderService : IOrderService
{
  private readonly IUnitOfWork _unitOfWork;

  public OrderService(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
  {
    if (dto.Items == null || dto.Items.Count == 0)
      throw new ArgumentException("A rendelésnek legalább egy tételt tartalmaznia kell.");

    var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId);
    if (customer is null)
      throw new InvalidOperationException($"Nem található ügyfél ezzel az azonosítóval: {dto.CustomerId}");

    var order = new Order
    {
      CustomerId = dto.CustomerId,
      OrderDate = DateTime.UtcNow,
      Items = new List<OrderItem>()
    };

    foreach (var itemDto in dto.Items)
    {
      var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);
      if (product is null)
        throw new InvalidOperationException($"Nem található termék ezzel az azonosítóval: {itemDto.ProductId}");

      if (itemDto.Quantity <= 0)
        throw new ArgumentException($"A mennyiségnek pozitívnak kell lennie (termék: {product.Name}).");

      order.Items.Add(new OrderItem
      {
        ProductId = itemDto.ProductId,
        Quantity = itemDto.Quantity
      });
    }

    await _unitOfWork.Orders.AddAsync(order);
    await _unitOfWork.SaveChangesAsync();

    var createdOrder = await _unitOfWork.Orders.GetByIdWithDetailsAsync(order.Id);
    return createdOrder!.ToDto();
  }

  public async Task<InvoiceDto?> GenerateInvoiceAsync(int orderId)
  {
    var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId);
    if (order is null)
      return null;

    var invoice = new InvoiceDto
    {
      OrderId = order.Id,
      CustomerName = order.Customer.Name,
      Date = order.OrderDate,
      Items = order.Items.Select(item => new InvoiceItemDto
      {
        ProductName = item.Product.Name,
        Quantity = item.Quantity,
        UnitPrice = item.Product.UnitPrice,
        IsDiscountEligible = item.Product.IsDiscountEligible,
        IsFragile = item.Product.IsFragile
      }).ToList()
    };

    invoice.TotalAmount = invoice.Items.Sum(i => i.LineTotal);

    return invoice;
  }

  public async Task<IEnumerable<OrderDto>> GetAllAsync()
  {
    var orders = await _unitOfWork.Orders.GetAllWithDetailsAsync();
    return orders.Select(o => o.ToDto());
  }

  public async Task<OrderDto?> GetByIdAsync(int id)
  {
    var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(id);
    return order?.ToDto();
  }
}