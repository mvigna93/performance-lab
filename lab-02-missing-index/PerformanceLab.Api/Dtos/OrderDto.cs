using PerformanceLab.Api.Entities;

namespace PerformanceLab.Api.Dtos;

public sealed record OrderDto(
    int Id,
    int CustomerId,
    DateTime CreatedAt,
    OrderStatus Status,
    decimal Total);
