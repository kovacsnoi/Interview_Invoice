namespace Invoicing.Domain.Entities;

public class Customer
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Country { get; set; } = string.Empty;
  public string Address { get; set; } = string.Empty;

  // Navigation property
  public ICollection<Order> Orders { get; set; } = new List<Order>();
}