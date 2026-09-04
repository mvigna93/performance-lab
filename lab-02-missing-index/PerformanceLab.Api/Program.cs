using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PerformanceLab.Api.Data;
using PerformanceLab.Api.Diagnostics;
using PerformanceLab.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<QueryCounter>();
builder.Services.AddScoped<QueryCountingInterceptor>();
builder.Services.AddDbContext<PerformanceLabDbContext>((serviceProvider, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PerformanceLab"));
    options.AddInterceptors(serviceProvider.GetRequiredService<QueryCountingInterceptor>());
    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await DatabaseInitializer.InitializeAsync(app.Services, app.Lifetime.ApplicationStopping);
}

app.UseMiddleware<QueryCountMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
