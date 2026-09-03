using Microsoft.EntityFrameworkCore;
using PerformanceLab.Api.Data;
using PerformanceLab.Api.Dtos;
using PerformanceLab.Api.Entities;

namespace PerformanceLab.Api.Services;

public sealed class OrderService(PerformanceLabDbContext dbContext) : IOrderService
{
    public async Task<IReadOnlyList<OrderDto>> GetRecentOrdersAsync(CancellationToken cancellationToken)
    {
        var availableOrders = await dbContext.Orders
            .Where(order => order.Status != OrderStatus.Cancelled)
            .ToListAsync(cancellationToken);

        var selectedOrders = availableOrders
            .OrderByDescending(order => order.CreatedAt)
            .Take(100)
            .ToList();

        var result = new List<OrderDto>(selectedOrders.Count);

        foreach (var order in selectedOrders)
        {
            var customer = await dbContext.Customers
                .SingleAsync(value => value.Id == order.CustomerId, cancellationToken);

            var customerOrderCount = await dbContext.Orders
                .CountAsync(value => value.CustomerId == customer.Id, cancellationToken);

            var orderItems = await dbContext.OrderItems
                .Where(item => item.OrderId == order.Id)
                .ToListAsync(cancellationToken);

            var itemDtos = new List<OrderItemDto>(orderItems.Count);
            foreach (var item in orderItems)
            {
                var product = await dbContext.Products
                    .SingleAsync(value => value.Id == item.ProductId, cancellationToken);

                var gross = item.UnitPrice * item.Quantity;
                var discount = gross * item.DiscountPercent / 100m;
                itemDtos.Add(new OrderItemDto(
                    item.Id,
                    item.Quantity,
                    item.UnitPrice,
                    item.DiscountPercent,
                    gross - discount,
                    new ProductDto(product.Id, product.Sku, product.Name, product.Category)));
            }

            var subtotal = orderItems.Sum(item => item.UnitPrice * item.Quantity);
            var discountTotal = orderItems.Sum(item =>
                item.UnitPrice * item.Quantity * item.DiscountPercent / 100m);
            var total = itemDtos.Select(item => item.LineTotal).Sum();

            result.Add(new OrderDto(
                order.Id,
                order.OrderNumber,
                order.CreatedAt,
                order.Status.ToString(),
                order.ShippingAddress,
                new CustomerDto(
                    customer.Id,
                    $"{customer.FirstName} {customer.LastName}",
                    customer.Email,
                    customer.City,
                    customerOrderCount),
                itemDtos,
                subtotal,
                discountTotal,
                total));
        }

        return result;
    }
}
