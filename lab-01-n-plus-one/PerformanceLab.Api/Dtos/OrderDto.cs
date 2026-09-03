namespace PerformanceLab.Api.Dtos;

public sealed record OrderDto(
    int Id,
    string OrderNumber,
    DateTime CreatedAt,
    string Status,
    string ShippingAddress,
    CustomerDto Customer,
    IReadOnlyList<OrderItemDto> Items,
    decimal Subtotal,
    decimal DiscountTotal,
    decimal Total);

public sealed record CustomerDto(int Id, string Name, string Email, string City, int OrderCount);

public sealed record OrderItemDto(
    int Id,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercent,
    decimal LineTotal,
    ProductDto Product);

public sealed record ProductDto(int Id, string Sku, string Name, string Category);
