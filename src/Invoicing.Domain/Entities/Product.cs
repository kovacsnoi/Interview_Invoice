namespace Invoicing.Domain.Entities;

public class Product
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Category { get; set; } = string.Empty;
  public decimal UnitPrice { get; set; }
  public bool IsHazardous { get; set; }
  public bool IsFragile { get; set; }
  public bool IsDiscountEligible { get; set; }

  // Navigation property
  public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}