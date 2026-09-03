using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PerformanceLab.Api.Data;

public sealed class PerformanceLabDbContextFactory : IDesignTimeDbContextFactory<PerformanceLabDbContext>
{
    public PerformanceLabDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PerformanceLabDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=performance_lab;Username=postgres;Password=postgres")
            .Options;

        return new PerformanceLabDbContext(options);
    }
}
