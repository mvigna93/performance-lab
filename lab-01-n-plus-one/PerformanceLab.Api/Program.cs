using Microsoft.EntityFrameworkCore;
using PerformanceLab.Api.Data;
using PerformanceLab.Api.Diagnostics;
using PerformanceLab.Api.Middleware;
using PerformanceLab.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<QueryCounter>();
builder.Services.AddScoped<QueryCountingInterceptor>();
builder.Services.AddDbContext<PerformanceLabDbContext>((serviceProvider, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PerformanceLab"));
    options.AddInterceptors(serviceProvider.GetRequiredService<QueryCountingInterceptor>());
    options.EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await DatabaseInitializer.InitializeAsync(app.Services);
}

app.UseHttpsRedirection();
app.UseMiddleware<QueryCountMiddleware>();
app.MapControllers();

app.Run();
