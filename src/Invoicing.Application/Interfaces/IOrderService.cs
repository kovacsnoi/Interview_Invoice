namespace Invoicing.Application.Interfaces;

using Invoicing.Application.Models;
using Invoicing.Domain.Entities;

public interface IOrderService
{
  Task<Order> CreateOrderAsync(CreateOrderDto dto);
  Task<InvoiceDto?> GenerateInvoiceAsync(int orderId);
}