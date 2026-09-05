namespace Invoicing.Application.Interfaces;

using Invoicing.Application.Models;
using Invoicing.Domain.Entities;

public interface IOrderService
{
  Task<IEnumerable<OrderDto>> GetAllAsync();
  Task<OrderDto?> GetByIdAsync(int id);
  Task<Order> CreateOrderAsync(CreateOrderDto dto);
  Task<InvoiceDto?> GenerateInvoiceAsync(int orderId);
}