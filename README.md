# TL;DR (Quick Overview)

Speculo is a backend system that:
*   Stores personal life data as immutable events instead of overwriting it.
*   Separates write logic from analytics logic (CQRS).
*   Generates insights such as trends and correlations from historical data.

**Why this matters:** Traditional CRUD systems lose history. Speculo preserves it, enabling replayable analytics and long-term behavioral insights.

**Tech stack:** .NET 9, PostgreSQL, CQRS, Clean Architecture, MediatR, Docker, Kubernetes.

---

## What Problem Does Speculo Solve?

Most tracking apps store only the latest state.

**Example (traditional app):**
*   Mood today = 7 (Yesterday’s mood is overwritten or poorly tracked).

**Speculo stores the full history instead:**
1.  Mood logged: 5
2.  Mood logged: 8
3.  Mood logged: 3

From this history, the system can compute:
*   Trends over time.
*   Averages and patterns.
*   Correlations between decisions and outcomes.

Speculo treats personal data as a timeline of events rather than mutable records.

---

## Core Architecture (Simple Explanation)

Speculo separates writing data from reading data.

### Write Side (Commands)
Responsible for validating actions and storing them as events.

**Examples:**
*   Log mood
*   Record decision (planned)
*   Register outcome (planned)

Each action is stored as an immutable event in PostgreSQL.

### Read Side (Queries)
Responsible for analytics and visualization.

**Current approach:**
*   On-the-fly projections computed from the event stream.

**Future direction:**
*   Persistent read models optimized for complex analytics.

This separation keeps business logic clean and analytics scalable.

---

## Key Design Decisions

**Why event-based storage instead of CRUD?**
CRUD overwrites data and loses history. Event-based storage preserves context and enables historical replay and evolving analytics.

**Why CQRS?**
Commands and queries have different requirements. Separating them reduces coupling and keeps analytics independent from validation logic.

**Why PostgreSQL?**
PostgreSQL provides strong transactional guarantees, JSON support, and efficient indexing—suitable for both event storage and analytical queries.

**Why on-the-fly projections initially?**
They keep the system simple during early development while maintaining a clear path toward scalable read models.

---

## Event Store Model

Events are stored in an append-only structure.

**Each event contains:**
*   `Id` — unique identifier
*   `UserId` — aggregate owner
*   `Type` — event type (e.g., `MoodLogged`)
*   `Timestamp` — occurrence time
*   `Payload` — JSON data

**Implementation notes:**
*   Events are immutable and append-only.
*   Ordering is enforced per user.
*   JSON payloads allow schema evolution without breaking historical data.

---

## Technology Stack

*   **Language & Framework**: C# / .NET 9
*   **Database**: PostgreSQL (Entity Framework Core)
*   **Architecture**: Clean Architecture
*   **CQRS**: MediatR
*   **Authentication**: JWT + BCrypt
*   **Containerization**: Docker
*   **Orchestration**: Kubernetes 

---

## Project Structure

```text
Speculo.Domain         // Core domain models and events
Speculo.Application    // Commands, queries, handlers
Speculo.Infrastructure // Persistence, event store, authentication
Speculo.API            // REST API layer
Speculo.Tests          // Unit and integration tests
```

---

## Example Use Cases

*   Track mood trends over time.
*   Replay historical data to apply new analytics logic.
*   Analyze correlations between decisions and outcomes.

---

## Project Goals

Speculo is not just a tracker app. It is an exploration of:
*   Event-based system design.
*   Analytics-oriented data modeling.
*   Separation of concerns in backend architecture.
*   Production-inspired patterns applied to a personal domain.

---

## Why This Project Exists

Speculo was built to explore how personal data can be modeled as a timeline of events rather than mutable state.

It serves both as a functional personal tool and as a technical experiment in event-driven backend design.