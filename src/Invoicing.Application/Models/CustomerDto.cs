namespace Invoicing.Application.Models;

using System.ComponentModel.DataAnnotations;

public class CustomerDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Country { get; set; } = string.Empty;
  public string Address { get; set; } = string.Empty;
}

public class CreateCustomerDto
{
  [Required(ErrorMessage = "A név megadása kötelező.")]
  [StringLength(200, MinimumLength = 1, ErrorMessage = "A név 1 és 200 karakter között lehet.")]
  public string Name { get; set; } = string.Empty;

  [Required(ErrorMessage = "Az ország megadása kötelező.")]
  [StringLength(100, MinimumLength = 1, ErrorMessage = "Az ország 1 és 100 karakter között lehet.")]
  public string Country { get; set; } = string.Empty;

  [Required(ErrorMessage = "A cím megadása kötelező.")]
  [StringLength(300, MinimumLength = 1, ErrorMessage = "A cím 1 és 300 karakter között lehet.")]
  public string Address { get; set; } = string.Empty;
}