using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformanceLab.Api.Data;

namespace PerformanceLab.Api.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthController(PerformanceLabDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var connected = await dbContext.Database.CanConnectAsync(cancellationToken);
        return connected
            ? Ok(new { status = "Healthy", database = "Connected" })
            : StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { status = "Unhealthy", database = "Unavailable" });
    }
}
