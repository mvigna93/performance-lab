using Microsoft.AspNetCore.Mvc;
using PerformanceLab.Api.Dtos;
using PerformanceLab.Api.Services;

namespace PerformanceLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<OrderDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> Get(CancellationToken cancellationToken)
    {
        var orders = await orderService.GetRecentOrdersAsync(cancellationToken);
        return Ok(orders);
    }
}
