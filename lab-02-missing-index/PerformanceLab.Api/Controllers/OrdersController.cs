using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformanceLab.Api.Data;
using PerformanceLab.Api.Dtos;
using PerformanceLab.Api.Entities;

namespace PerformanceLab.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(PerformanceLabDbContext dbContext) : ControllerBase
{
    [HttpGet("search")]
    [ProducesResponseType<List<OrderDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<OrderDto>>> Search(
        [FromQuery, Range(1, int.MaxValue)] int customerId,
        CancellationToken cancellationToken)
    {
        var orders = await dbContext.Orders
            .AsNoTracking()
            .Where(order => order.CustomerId == customerId
                && order.Status != OrderStatus.Cancelled)
            .OrderByDescending(order => order.CreatedAt)
            .Take(100)
            .Select(order => new OrderDto(
                order.Id,
                order.CustomerId,
                order.CreatedAt,
                order.Status,
                order.Total))
            .ToListAsync(cancellationToken);

        return Ok(orders);
    }
}
