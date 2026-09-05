namespace Invoicing.Domain.Entities;

public class Order
{
	public int Id { get; set; }
	public int CustomerId { get; set; }
	public DateTime OrderDate { get; set; }

	// Navigation properties
	public Customer Customer { get; set; } = null!;
	public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}