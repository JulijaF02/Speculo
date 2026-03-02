# Speculo

> A distributed lifestyle tracking platform built with .NET microservices, deployed on Azure Kubernetes Service.

**Live**: [http://speculo-app.eastus.cloudapp.azure.com](http://speculo-app.eastus.cloudapp.azure.com)

---

## What It Does

Speculo gives users a single dashboard to track and reflect on four pillars of daily life:

- **Mood** — Log emotional state on a 1-10 scale with notes
- **Sleep** — Record hours slept and sleep quality
- **Workouts** — Track exercise type, duration, and intensity
- **Finances** — Log income and expenses by category

Events flow through an event-driven pipeline and are projected into real-time analytics on the dashboard.

## Architecture

```
                    ┌─────────────┐
                    │   React UI  │
                    │   (Nginx)   │
                    └──────┬──────┘
                           │
              ┌────────────┼────────────┐
              │            │            │
      ┌───────▼──┐  ┌──────▼───┐  ┌────▼──────┐
      │ Identity │  │ Tracking │  │ Analytics │
      │  Service │  │  Service │  │  Service  │
      │ (Auth)   │  │ (Write)  │  │  (Read)   │
      └───┬──────┘  └──┬───┬──┘  └──┬────┬───┘
          │             │   │        │    │
      ┌───▼───┐   ┌────▼┐  │   ┌────▼┐ ┌─▼───┐
      │Postgres│   │Postgres  │MongoDB│ │Redis│
      └───────┘   └─────┘  │   └─────┘ └─────┘
                            │
                       ┌────▼────┐
                       │  Kafka  │
                       └────┬────┘
                            │
                       ┌────▼──────┐
                       │ Analytics │
                       │ Consumer  │
                       └───────────┘
```

### Services

| Service | Responsibility | Database | Port |
|---------|---------------|----------|------|
| **Identity** | Registration, login, JWT auth (BCrypt hashing) | PostgreSQL | 5001 |
| **Tracking** | Event ingestion, validation, command handling | PostgreSQL | 5000 |
| **Analytics** | Read projections, dashboard queries, caching | MongoDB + Redis | 5002 |

### Key Patterns

- **CQRS** — Commands go to Tracking (PostgreSQL), queries served by Analytics (MongoDB). Separate write and read models optimized for their workloads.
- **Event-Driven Architecture** — Tracking publishes domain events to Kafka. Analytics consumes them asynchronously to build materialized views. Services are fully decoupled.
- **Event-Driven Cache Invalidation** — Redis cache is invalidated when new events are consumed, ensuring dashboard data is fresh without polling.
- **API Gateway** — Nginx reverse proxy in the frontend container routes `/api/identity/`, `/api/tracking/`, `/api/analytics/` to backend services. Single entry point for the client.

## Tech Stack

| Layer | Technologies |
|-------|-------------|
| Backend | .NET 9, ASP.NET Core, Entity Framework Core, MediatR, FluentValidation |
| Frontend | React 19, TypeScript, Vite, Tailwind CSS |
| Messaging | Apache Kafka, ZooKeeper |
| Databases | PostgreSQL 17, MongoDB 7 |
| Caching | Redis 7 |
| Infrastructure | Docker, Kubernetes (AKS), Azure Container Registry, Nginx |
| CI | GitHub Actions (build + test on every PR) |

## Project Structure

```
├── Speculo.API/                    # Tracking Service (commands + events)
├── Speculo.Identity/               # Identity Service (auth + JWT)
├── Speculo.Analytics/              # Analytics Service (projections + queries)
├── Speculo.Application/            # Application layer (CQRS handlers, MediatR)
├── Speculo.Domain/                 # Domain entities and interfaces
├── Speculo.Infrastructure/         # EF Core, repositories, Kafka producer
├── Speculo.Contracts/              # Shared DTOs and event contracts
├── Speculo.Application.UnitTests/  # Unit tests (xUnit)
├── speculo-client/                 # React frontend
├── k8s/                            # Kubernetes manifests
├── docker-compose.yml              # Local development orchestration
├── Dockerfile                      # Tracking API image
├── Dockerfile.identity             # Identity API image
└── Dockerfile.analytics            # Analytics API image
```

## Getting Started

### Prerequisites
- Docker Desktop

### Run Locally
```bash
docker compose up -d --build
```

Open [http://localhost:3000](http://localhost:3000) — register an account, log some events, and check the dashboard.

### Run Tests
```bash
dotnet test
```

## Deployment

Production runs on **Azure Kubernetes Service (AKS)** with images stored in **Azure Container Registry**.

```bash
# Apply infrastructure (databases, Kafka, Redis)
kubectl apply -f k8s/stateful-services.yaml

# Deploy application services + frontend
kubectl apply -f k8s/apps.yaml
```

Kubernetes manifests are in `/k8s`. CI runs automatically via GitHub Actions on every push and PR to `main`.
