# Performance Lab

A collection of reproducible experiments focused on software performance, profiling, load testing, databases, observability, and reliability.

The goal of this repository is to investigate performance problems using a measurement-driven approach:

**Baseline → Measure → Profile → Identify Bottleneck → Optimize → Re-test**

## Labs

| Lab | Topic | Status |
| --- | --- | --- |
| [Lab 01](./lab-01-n-plus-one) | ASP.NET Core + EF Core N+1 query problem | Planned |

## Topics

- Application performance
- .NET profiling
- Database performance
- Load and stress testing
- Latency and throughput analysis
- Memory and CPU profiling
- Observability and distributed tracing
- Performance regression testing
- Software reliability

## Principles

Each experiment should be:

- Reproducible
- Measurable
- Based on real metrics
- Documented with before/after results
- Focused on understanding the root cause, not only the fix

## Tech Stack

The labs may use technologies such as:

- .NET / ASP.NET Core
- PostgreSQL
- Entity Framework Core
- k6
- OpenTelemetry
- Grafana
- Docker

## Repository Structure

```text
performance-lab/
├── lab-01-n-plus-one/
├── lab-02-...
└── README.md
