using Invoicing.Application.Mapping;
using Invoicing.Domain.Entities;
using Xunit;

namespace Invoicing.Application.Tests;

public class MappingExtensionsTests
{
  [Fact]
  public void ToDto_Product_MapsAllFieldsCorrectly()
  {
    var product = new Product
    {
      Id = 1,
      Name = "Laptop",
      Category = "Electronics",
      UnitPrice = 1000m,
      IsHazardous = false,
      IsFragile = true,
      IsDiscountEligible = false
    };

    var dto = product.ToDto();

    Assert.Equal(product.Id, dto.Id);
    Assert.Equal(product.Name, dto.Name);
    Assert.Equal(product.UnitPrice, dto.UnitPrice);
    Assert.Equal(product.IsFragile, dto.IsFragile);
  }

  [Fact]
  public void ToDto_Order_WithMissingNavigationProperties_DoesNotThrow()
  {
    var order = new Order
    {
      Id = 1,
      CustomerId = 1,
      Customer = null!,
      OrderDate = DateTime.UtcNow,
      Items = new List<OrderItem>
            {
                new() { ProductId = 1, Product = null!, Quantity = 2 }
            }
    };

    var dto = order.ToDto();

    Assert.Equal(string.Empty, dto.CustomerName);
    Assert.Equal(string.Empty, dto.Items[0].ProductName);
  }
}