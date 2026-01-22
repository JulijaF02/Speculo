# Speculo 

**Speculo** is a high-performance, event-driven backend system designed for extensible mood tracking and life-event logging. Built with **ASP.NET Core** and **Clean Architecture**, it serves as a demonstration of modern software craftsmanship, emphasizing maintainability, scalability, and security.

---

## 🏗 Architecture & Design
The project follows **Clean Architecture (Onion)** to ensure the business logic remains independent of external frameworks and databases.

*   **CQRS Pattern**: Decoupling read and write operations using **MediatR**.
*   **Event-Driven Data Model**: Instead of static tables, Speculo uses a generic [Event](cci:2://file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Domain/Entities/Event.cs:4:0-11:1) store to capture state changes, providing a flexible foundation for future analytics or microservices.
*   **Dependency Injection**: Extensive use of the Options Pattern and interface-based design for high testability.

## 🛠 Tech Stack
*   **Framework**: .NET 8/9
*   **Persistence**: PostgreSQL with Entity Framework Core
*   **Security**: JWT Bearer Authentication & BCrypt Password Hashing
*   **Containerization**: Docker-ready for cloud-native deployment
*   **Communication**: MediatR for clean in-process messaging

## 🚀 Key Features
- **Secure Identity**: End-to-end authentication flow with custom JWT generation.
- **Flexible Event Store**: Log any user activity (Mood, Habit, Health) via a unified API endpoint without schema changes.
- **Clean API**: Fully documented endpoints with proper HTTP status codes and standardized response models.
- **Robustness**: Organized folder structure following industry standard naming conventions and separation of concerns.

## 📂 Project Structure
- `Speculo.Domain`: Pure POCO entities and core business rules.
- `Speculo.Application`: MediatR handlers, DTOs, and interface definitions.
- `Speculo.Infrastructure`: Implementation of persistence (EF Core Context) and external services (Identity).
- `Speculo.API`: Thin controllers and startup configuration.

## 🗺️ Project Roadmap
- [x] Core Backend API (Clean Architecture)
- [x] JWT Authentication & Identity
- [x] Event Store Persistence
- [ ] **Frontend Dashboard (Vue.js / React)** - *In Progress*
- [ ] Data Visualization (Mood Trends & Analytics)
- [ ] Docker & AWS Deployment

---
*Developed as a portfolio project to explore event-driven systems and enterprise-grade software design.*