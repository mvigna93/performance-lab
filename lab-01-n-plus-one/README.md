# Lab 01 — Order API

An ASP.NET Core and PostgreSQL order-management API for performance investigation. The orders endpoint returns 100 recent non-cancelled orders with customer details, line items, product details, and calculated totals.

## Prerequisites

- .NET 9 SDK
- Docker Desktop, or a PostgreSQL instance available on port 5432

## Run

From `lab-01-n-plus-one`:

```powershell
docker compose up -d
dotnet run --project .\PerformanceLab.Api\PerformanceLab.Api.csproj --launch-profile http
```

In Development, the application applies its EF Core migration and seeds the database on the first startup. Initial seeding creates 500 customers, 5,000 products, 10,000 orders, and multiple items for each order, so the first launch can take a little longer.

Open:

- Swagger UI: `http://localhost:5247/swagger`
- Orders: `http://localhost:5247/api/orders`
- Health: `http://localhost:5247/health`

The default connection string is in `PerformanceLab.Api/appsettings.json`. Override `ConnectionStrings__PerformanceLab` to use another PostgreSQL instance.

## Build

```powershell
dotnet build .\PerformanceLab.sln
```
To create a basic txt file with results, execute this command: k6 run .\k6\orders-baseline.js | Tee-Object .\results\nameYouWish.txt
