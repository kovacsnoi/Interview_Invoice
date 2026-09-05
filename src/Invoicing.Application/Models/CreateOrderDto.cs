namespace Invoicing.Application.Models;

using System.ComponentModel.DataAnnotations;

public class CreateOrderDto
{
  [Range(1, int.MaxValue, ErrorMessage = "A CustomerId-nek pozitívnak kell lennie.")]
  public int CustomerId { get; set; }

  [MinLength(1, ErrorMessage = "A rendelésnek legalább egy tételt tartalmaznia kell.")]
  public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
  [Range(1, int.MaxValue, ErrorMessage = "A ProductId-nek pozitívnak kell lennie.")]
  public int ProductId { get; set; }

  [Range(1, int.MaxValue, ErrorMessage = "A mennyiségnek pozitívnak kell lennie.")]
  public int Quantity { get; set; }
}