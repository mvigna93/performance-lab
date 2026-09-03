using Microsoft.EntityFrameworkCore;
using PerformanceLab.Api.Data;
using PerformanceLab.Api.Dtos;
using PerformanceLab.Api.Entities;

namespace PerformanceLab.Api.Services;

public sealed class OrderService(PerformanceLabDbContext dbContext) : IOrderService
{
    public async Task<IReadOnlyList<OrderDto>> GetRecentOrdersAsync(
        CancellationToken cancellationToken)
    {
        var orders = await dbContext.Orders
            .AsNoTracking()
            .Where(order => order.Status != OrderStatus.Cancelled)
            .OrderByDescending(order => order.CreatedAt)
            .ThenByDescending(order => order.Id)
            .Take(100)
            .Select(order => new
            {
                order.Id,
                order.OrderNumber,
                order.CreatedAt,
                order.Status,
                order.ShippingAddress,
                CustomerId = order.Customer.Id,
                CustomerName = order.Customer.FirstName + " " + order.Customer.LastName,
                CustomerEmail = order.Customer.Email,
                CustomerCity = order.Customer.City,
                CustomerOrderCount = order.Customer.Orders.Count
            })
            .ToListAsync(cancellationToken);

        if (orders.Count == 0)
        {
            return [];
        }

        var orderIds = orders.Select(order => order.Id).ToArray();
        var items = await dbContext.OrderItems
            .AsNoTracking()
            .Where(item => orderIds.Contains(item.OrderId))
            .OrderBy(item => item.Id)
            .Select(item => new
            {
                item.OrderId,
                item.Id,
                item.Quantity,
                item.UnitPrice,
                item.DiscountPercent,
                ProductId = item.Product.Id,
                ProductSku = item.Product.Sku,
                ProductName = item.Product.Name,
                ProductCategory = item.Product.Category
            })
            .ToListAsync(cancellationToken);

        var itemsByOrder = items.ToLookup(item => item.OrderId);
        var result = new List<OrderDto>(orders.Count);

        foreach (var order in orders)
        {
            var orderItems = itemsByOrder[order.Id];
            var itemDtos = new List<OrderItemDto>();
            var subtotal = 0m;
            var discountTotal = 0m;

            foreach (var item in orderItems)
            {
                var gross = item.UnitPrice * item.Quantity;
                var discount = gross * item.DiscountPercent / 100m;

                subtotal += gross;
                discountTotal += discount;
                itemDtos.Add(new OrderItemDto(
                    item.Id,
                    item.Quantity,
                    item.UnitPrice,
                    item.DiscountPercent,
                    gross - discount,
                    new ProductDto(
                        item.ProductId,
                        item.ProductSku,
                        item.ProductName,
                        item.ProductCategory)));
            }

            result.Add(new OrderDto(
                order.Id,
                order.OrderNumber,
                order.CreatedAt,
                order.Status.ToString(),
                order.ShippingAddress,
                new CustomerDto(
                    order.CustomerId,
                    order.CustomerName,
                    order.CustomerEmail,
                    order.CustomerCity,
                    order.CustomerOrderCount),
                itemDtos,
                subtotal,
                discountTotal,
                subtotal - discountTotal));
        }

        return result;
    }
}
