using System.Globalization;
using PerformanceLab.Api.Diagnostics;

namespace PerformanceLab.Api.Middleware;

public sealed class QueryCountMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, QueryCounter queryCounter)
    {
        queryCounter.Reset();
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Db-Queries"] = queryCounter.Count.ToString(CultureInfo.InvariantCulture);
            return Task.CompletedTask;
        });

        await next(context);
    }
}
