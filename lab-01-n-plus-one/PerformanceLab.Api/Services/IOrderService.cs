using PerformanceLab.Api.Dtos;

namespace PerformanceLab.Api.Services;

public interface IOrderService
{
    Task<IReadOnlyList<OrderDto>> GetRecentOrdersAsync(CancellationToken cancellationToken);
}
