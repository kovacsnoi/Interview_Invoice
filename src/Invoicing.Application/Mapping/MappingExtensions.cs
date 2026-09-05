namespace Invoicing.Application.Mapping;

using Invoicing.Application.Models;
using Invoicing.Domain.Entities;

public static class MappingExtensions
{
  public static ProductDto ToDto(this Product product) => new()
  {
    Id = product.Id,
    Name = product.Name,
    Category = product.Category,
    UnitPrice = product.UnitPrice,
    IsHazardous = product.IsHazardous,
    IsFragile = product.IsFragile,
    IsDiscountEligible = product.IsDiscountEligible
  };

  public static Product ToEntity(this CreateProductDto dto) => new()
  {
    Name = dto.Name,
    Category = dto.Category,
    UnitPrice = dto.UnitPrice,
    IsHazardous = dto.IsHazardous,
    IsFragile = dto.IsFragile,
    IsDiscountEligible = dto.IsDiscountEligible
  };

  public static CustomerDto ToDto(this Customer customer) => new()
  {
    Id = customer.Id,
    Name = customer.Name,
    Country = customer.Country,
    Address = customer.Address
  };

  public static Customer ToEntity(this CreateCustomerDto dto) => new()
  {
    Name = dto.Name,
    Country = dto.Country,
    Address = dto.Address
  };

  public static OrderDto ToDto(this Order order) => new()
  {
    Id = order.Id,
    CustomerId = order.CustomerId,
    CustomerName = order.Customer?.Name ?? string.Empty,
    OrderDate = order.OrderDate,
    Items = order.Items.Select(i => new OrderItemDto
    {
      ProductId = i.ProductId,
      ProductName = i.Product?.Name ?? string.Empty,
      Quantity = i.Quantity,
      UnitPrice = i.Product?.UnitPrice ?? 0
    }).ToList()
  };
}