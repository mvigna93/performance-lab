# Lab 01 — N+1 Query Problem in ASP.NET Core

## Goal

Investigate how an N+1 database query problem affects API latency and throughput.

## Scenario

An ASP.NET Core API exposes an endpoint that returns orders together with customer and order-item information.

The initial implementation will intentionally generate unnecessary database queries.

## Metrics

- Average latency
- p95 latency
- p99 latency
- Throughput
- Number of database queries per request

## Experiment

1. Build the intentionally inefficient implementation.
2. Establish a performance baseline.
3. Inspect database activity.
4. Identify the bottleneck.
5. Optimize the implementation.
6. Run the same workload again.
7. Compare the results.

## Stack

- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- k6
