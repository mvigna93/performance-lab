namespace PerformanceLab.Api.Entities;

public sealed class Order
{
    public int Id { get; set; }
    public required string OrderNumber { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public required string ShippingAddress { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}
