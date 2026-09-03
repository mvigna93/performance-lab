namespace PerformanceLab.Api.Entities;

public sealed class Product
{
    public int Id { get; set; }
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
