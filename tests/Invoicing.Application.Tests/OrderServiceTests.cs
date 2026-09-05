using Invoicing.Application.Interfaces;
using Invoicing.Application.Models;
using Invoicing.Application.Services;
using Invoicing.Domain.Entities;
using Invoicing.Application.Exceptions;
using Moq;
using Xunit;

namespace Invoicing.Application.Tests;

public class OrderServiceTests
{
  private readonly Mock<IUnitOfWork> _unitOfWorkMock;
  private readonly Mock<ICustomerRepository> _customerRepoMock;
  private readonly Mock<IProductRepository> _productRepoMock;
  private readonly Mock<IOrderRepository> _orderRepoMock;
  private readonly OrderService _sut;

  public OrderServiceTests()
  {
    _unitOfWorkMock = new Mock<IUnitOfWork>();
    _customerRepoMock = new Mock<ICustomerRepository>();
    _productRepoMock = new Mock<IProductRepository>();
    _orderRepoMock = new Mock<IOrderRepository>();

    _unitOfWorkMock.Setup(u => u.Customers).Returns(_customerRepoMock.Object);
    _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
    _unitOfWorkMock.Setup(u => u.Orders).Returns(_orderRepoMock.Object);

    _sut = new OrderService(_unitOfWorkMock.Object);
  }

  [Fact]
  public async Task CreateOrderAsync_WithEmptyItems_ThrowsArgumentException()
  {
    var dto = new CreateOrderDto { CustomerId = 1, Items = new List<CreateOrderItemDto>() };

    await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateOrderAsync(dto));
  }

  [Fact]
  public async Task CreateOrderAsync_WithNonExistentCustomer_ThrowsNotFoundException()
  {
    _customerRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
        .ReturnsAsync((Customer?)null);

    var dto = new CreateOrderDto
    {
      CustomerId = 999,
      Items = new List<CreateOrderItemDto> { new() { ProductId = 1, Quantity = 1 } }
    };

    await Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateOrderAsync(dto));
  }

  [Fact]
  public async Task CreateOrderAsync_WithNonExistentProduct_ThrowsNotFoundException()
  {
    _customerRepoMock.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new Customer { Id = 1, Name = "Test Kft", Country = "HU", Address = "Cím" });
    _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
        .ReturnsAsync((Product?)null);

    var dto = new CreateOrderDto
    {
      CustomerId = 1,
      Items = new List<CreateOrderItemDto> { new() { ProductId = 999, Quantity = 1 } }
    };

    await Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateOrderAsync(dto));
  }

  [Fact]
  public async Task CreateOrderAsync_WithZeroOrNegativeQuantity_ThrowsArgumentException()
  {
    _customerRepoMock.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new Customer { Id = 1, Name = "Test Kft", Country = "HU", Address = "Cím" });
    _productRepoMock.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new Product { Id = 1, Name = "Laptop", Category = "Electronics", UnitPrice = 1000m });

    var dto = new CreateOrderDto
    {
      CustomerId = 1,
      Items = new List<CreateOrderItemDto> { new() { ProductId = 1, Quantity = 0 } }
    };

    await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateOrderAsync(dto));
  }

  [Fact]
  public async Task CreateOrderAsync_WithValidData_ReturnsOrderDtoAndCallsSaveChanges()
  {
    var customer = new Customer { Id = 1, Name = "ABC Ltd", Country = "HU", Address = "Cím" };
    var product = new Product { Id = 1, Name = "Laptop", Category = "Electronics", UnitPrice = 1000m };

    _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);
    _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

    var savedOrder = new Order
    {
      Id = 1,
      CustomerId = 1,
      Customer = customer,
      OrderDate = DateTime.UtcNow,
      Items = new List<OrderItem>
            {
                new() { ProductId = 1, Product = product, Quantity = 2 }
            }
    };
    _orderRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>()))
        .ReturnsAsync(savedOrder);

    var dto = new CreateOrderDto
    {
      CustomerId = 1,
      Items = new List<CreateOrderItemDto> { new() { ProductId = 1, Quantity = 2 } }
    };

    var result = await _sut.CreateOrderAsync(dto);

    Assert.NotNull(result);
    Assert.Equal(1, result.CustomerId);
    Assert.Single(result.Items);
    Assert.Equal(2, result.Items[0].Quantity);
    _orderRepoMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
    _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
  }

  [Fact]
  public async Task GenerateInvoiceAsync_WithNonExistentOrder_ReturnsNull()
  {
    _orderRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>()))
        .ReturnsAsync((Order?)null);

    var result = await _sut.GenerateInvoiceAsync(999);

    Assert.Null(result);
  }

  [Fact]
  public async Task GenerateInvoiceAsync_CalculatesTotalAmountCorrectly()
  {
    var order = new Order
    {
      Id = 1,
      Customer = new Customer { Id = 1, Name = "ABC Ltd", Country = "HU", Address = "Cím" },
      OrderDate = DateTime.UtcNow,
      Items = new List<OrderItem>
            {
                new() { ProductId = 1, Product = new Product { Name = "Laptop", UnitPrice = 1000m, IsDiscountEligible = false, IsFragile = true }, Quantity = 2 },
                new() { ProductId = 2, Product = new Product { Name = "Mouse", UnitPrice = 20m, IsDiscountEligible = true, IsFragile = false }, Quantity = 3 }
            }
    };
    _orderRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(order);

    var result = await _sut.GenerateInvoiceAsync(1);

    Assert.NotNull(result);
    Assert.Equal(2060m, result!.TotalAmount);
  }
}