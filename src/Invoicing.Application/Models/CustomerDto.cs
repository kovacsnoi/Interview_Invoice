namespace Invoicing.Application.Models;

public class CustomerDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Country { get; set; } = string.Empty;
  public string Address { get; set; } = string.Empty;
}

public class CreateCustomerDto
{
  public string Name { get; set; } = string.Empty;
  public string Country { get; set; } = string.Empty;
  public string Address { get; set; } = string.Empty;
}