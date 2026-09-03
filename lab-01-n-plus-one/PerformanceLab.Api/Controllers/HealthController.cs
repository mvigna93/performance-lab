using Microsoft.AspNetCore.Mvc;
using PerformanceLab.Api.Data;

namespace PerformanceLab.Api.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthController(PerformanceLabDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
        return canConnect
            ? Ok(new { status = "Healthy", database = "Connected" })
            : StatusCode(StatusCodes.Status503ServiceUnavailable, new { status = "Unhealthy", database = "Unavailable" });
    }
}
