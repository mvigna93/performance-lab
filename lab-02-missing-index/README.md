# Lab 02 — Missing Index

## Experiment objective

Investigate the database access behavior of a customer order search while the application uses a bounded, read-only EF Core query. Query counts are exposed through the `X-Db-Queries` response header.

## Prerequisites

- .NET 9 SDK
- Docker Desktop with the Linux container engine running
- k6
- Available ports 5433 (PostgreSQL) and 5248 (API)

## Startup instructions

From the repository root:

```powershell
cd lab-02-missing-index
docker compose up -d --wait
dotnet run --project .\PerformanceLab.Api\PerformanceLab.Api.csproj --launch-profile http
```

Development startup creates the initial schema using EF Core `EnsureCreatedAsync` and seeds 10,000 customers and 1,000,000 orders. Wait for the API's listening message before testing. This lab does not use migrations.

The seed uses fixed timestamps and deterministic arithmetic, with set-based PostgreSQL inserts in one transaction. Each customer has 100 orders, including 20 cancelled orders. A new empty database produces the same records. Subsequent starts reuse the data; existing populated databases are not overwritten.

The connection string is in `PerformanceLab.Api/appsettings.json`; it can be overridden with `ConnectionStrings__PerformanceLab`. The Compose project and volume are separate from Lab 01. Credentials are for local development only.

Swagger: [http://localhost:5248/swagger](http://localhost:5248/swagger)

Health: [http://localhost:5248/health](http://localhost:5248/health)

## Reproduction instructions

In a second terminal, from `lab-02-missing-index`:

```powershell
curl.exe -i "http://localhost:5248/api/orders/search?customerId=42"
k6 run --summary-export .\results\initial-summary.json .\k6\orders-search.js
```

The endpoint returns up to 100 non-cancelled orders for the specified customer, newest first. With the initial seed, a valid customer returns 80 orders. Customer IDs 1–10,000 are populated. Non-positive or missing IDs return HTTP 400; an unknown positive ID returns an empty list.

The k6 workload uses 10 virtual users for 30 seconds, cycles deterministically through customer IDs, checks HTTP 200, and sleeps one second per iteration. It has no performance thresholds. The summary is written to `results/initial-summary.json`.

The search response should include `X-Db-Queries: 1`. Startup commands use a separate dependency-injection scope and are not included. EF Core SQL commands and parameters are logged in Development.

## Intentionally missing index

The initial `Orders` table has only its primary-key index, `PK_Orders`, on `Id`. No secondary index is defined on `CustomerId`, `CreatedAt`, or `Status`, individually or in combination.

The foreign key to `Customers` remains enforced. The EF Core convention that automatically adds foreign-key indexes is disabled for this lab so it does not introduce an unrequested secondary index.
