using Microsoft.EntityFrameworkCore;
using PerformanceLab.Api.Entities;

namespace PerformanceLab.Api.Data;

public static class DatabaseInitializer
{
    private static readonly string[] FirstNames = ["Emma", "Liam", "Olivia", "Noah", "Ava", "Elijah", "Mia", "Lucas", "Sofia", "Leo"];
    private static readonly string[] LastNames = ["Martin", "Rossi", "Taylor", "Schmidt", "Silva", "Dubois", "Nielsen", "Kowalski", "Garcia", "Novak"];
    private static readonly string[] Cities = ["Milan", "Paris", "Berlin", "Madrid", "Lisbon", "Vienna", "Prague", "Dublin", "Oslo", "Amsterdam"];
    private static readonly string[] Categories = ["Computers", "Office", "Audio", "Networking", "Storage", "Accessories", "Displays", "Mobile"];

    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PerformanceLabDbContext>();

        await dbContext.Database.MigrateAsync();

        if (await dbContext.Customers.AnyAsync())
        {
            return;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        var random = new Random(1874);
        var createdAt = DateTime.UtcNow.Date.AddYears(-2);

        var customers = Enumerable.Range(1, 500)
            .Select(id => new Customer
            {
                Id = id,
                FirstName = FirstNames[(id - 1) % FirstNames.Length],
                LastName = LastNames[((id - 1) / FirstNames.Length) % LastNames.Length],
                Email = $"customer{id:D4}@example.test",
                City = Cities[(id - 1) % Cities.Length],
                CreatedAt = createdAt.AddDays(random.Next(0, 700))
            })
            .ToList();

        var products = Enumerable.Range(1, 5_000)
            .Select(id => new Product
            {
                Id = id,
                Sku = $"SKU-{id:D6}",
                Name = $"{Categories[(id - 1) % Categories.Length]} Product {id:D4}",
                Category = Categories[(id - 1) % Categories.Length],
                Price = decimal.Round(5m + (decimal)random.NextDouble() * 995m, 2),
                IsActive = id % 29 != 0
            })
            .ToList();

        dbContext.Customers.AddRange(customers);
        dbContext.Products.AddRange(products);
        await dbContext.SaveChangesAsync();

        var nextOrderItemId = 1;
        for (var firstOrderId = 1; firstOrderId <= 10_000; firstOrderId += 500)
        {
            var orders = new List<Order>(500);
            var orderItems = new List<OrderItem>(2_500);

            for (var orderId = firstOrderId; orderId < firstOrderId + 500; orderId++)
            {
                var customerId = random.Next(1, customers.Count + 1);
                var order = new Order
                {
                    Id = orderId,
                    OrderNumber = $"ORD-{createdAt.Year}-{orderId:D7}",
                    CustomerId = customerId,
                    CreatedAt = createdAt.AddMinutes(random.Next(0, 1_050_000)),
                    Status = (OrderStatus)random.Next(0, 5),
                    ShippingAddress = $"{random.Next(1, 250)} Market Street, {customers[customerId - 1].City}"
                };
                orders.Add(order);

                var itemCount = random.Next(2, 7);
                for (var itemNumber = 0; itemNumber < itemCount; itemNumber++)
                {
                    var productId = random.Next(1, products.Count + 1);
                    orderItems.Add(new OrderItem
                    {
                        Id = nextOrderItemId++,
                        OrderId = orderId,
                        ProductId = productId,
                        Quantity = random.Next(1, 6),
                        UnitPrice = products[productId - 1].Price,
                        DiscountPercent = random.Next(0, 5) == 0 ? random.Next(5, 26) : 0
                    });
                }
            }

            dbContext.Orders.AddRange(orders);
            dbContext.OrderItems.AddRange(orderItems);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();
        }

        await transaction.CommitAsync();
    }
}
