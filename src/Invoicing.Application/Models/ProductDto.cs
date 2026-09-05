namespace Invoicing.Application.Models;

using System.ComponentModel.DataAnnotations;

public class ProductDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Category { get; set; } = string.Empty;
  public decimal UnitPrice { get; set; }
  public bool IsHazardous { get; set; }
  public bool IsFragile { get; set; }
  public bool IsDiscountEligible { get; set; }
}

public class CreateProductDto
{
  [Required(ErrorMessage = "A név megadása kötelező.")]
  [StringLength(200, MinimumLength = 1, ErrorMessage = "A név 1 és 200 karakter között lehet.")]
  public string Name { get; set; } = string.Empty;

  [Required(ErrorMessage = "A kategória megadása kötelező.")]
  [StringLength(100, MinimumLength = 1, ErrorMessage = "A kategória 1 és 100 karakter között lehet.")]
  public string Category { get; set; } = string.Empty;

  [Range(0.01, 1_000_000, ErrorMessage = "Az árnak 0.01 és 1 000 000 közé kell esnie.")]
  public decimal UnitPrice { get; set; }

  public bool IsHazardous { get; set; }
  public bool IsFragile { get; set; }
  public bool IsDiscountEligible { get; set; }
}