using Microsoft.EntityFrameworkCore;

namespace PerformanceLab.Api.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PerformanceLabDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(DatabaseInitializer));

        dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

        await dbContext.Database.ExecuteSqlRawAsync(
            "SELECT pg_advisory_xact_lock(2002);", cancellationToken);

        if (await dbContext.Customers.AnyAsync(cancellationToken))
        {
            await transaction.CommitAsync(cancellationToken);
            return;
        }

        logger.LogInformation("Seeding 10,000 customers and 1,000,000 orders.");

        await dbContext.Database.ExecuteSqlRawAsync("""
            INSERT INTO "Customers" ("Id", "Name", "Email")
            SELECT n,
                   'Customer ' || lpad(n::text, 5, '0'),
                   'customer' || n || '@example.test'
            FROM generate_series(1, 10000) AS series(n);

            INSERT INTO "Orders" ("Id", "CustomerId", "CreatedAt", "Status", "Total")
            SELECT n,
                   ((n - 1) % 10000) + 1,
                   TIMESTAMPTZ '2024-01-01 00:00:00+00'
                       + ((n::bigint * 7919) % 63072000) * INTERVAL '1 second',
                   CASE ((n - 1) / 10000) % 5
                       WHEN 0 THEN 'Pending'
                       WHEN 1 THEN 'Processing'
                       WHEN 2 THEN 'Shipped'
                       WHEN 3 THEN 'Delivered'
                       ELSE 'Cancelled'
                   END,
                   (1000 + ((n::bigint * 3571) % 199000))::numeric / 100
            FROM generate_series(1, 1000000) AS series(n);

            SELECT setval(pg_get_serial_sequence('"Customers"', 'Id'), 10000, true);
            SELECT setval(pg_get_serial_sequence('"Orders"', 'Id'), 1000000, true);
            """, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        logger.LogInformation("Database seeding completed.");
    }
}
