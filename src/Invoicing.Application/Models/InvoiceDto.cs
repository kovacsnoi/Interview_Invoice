namespace Invoicing.Application.Models;

public class InvoiceDto
{
  public int OrderId { get; set; }
  public string CustomerName { get; set; } = string.Empty;
  public DateTime Date { get; set; }
  public List<InvoiceItemDto> Items { get; set; } = new();
  public decimal TotalAmount { get; set; }
}

public class InvoiceItemDto
{
  public string ProductName { get; set; } = string.Empty;
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
  public decimal LineTotal => Quantity * UnitPrice;

  public bool IsDiscountEligible { get; set; }
  public bool IsFragile { get; set; }

  public string FormattedProductName
  {
    get
    {
      var tags = new List<string>();
      if (IsDiscountEligible) tags.Add("[KEDVEZMÉNYES]");
      if (IsFragile) tags.Add("[TÖRÉKENY]");

      return tags.Count > 0
          ? $"{ProductName} {string.Join(" ", tags)}"
          : ProductName;
    }
  }
}