# Speculo

**Live Link**: [http://speculo-app.eastus.cloudapp.azure.com](http://speculo-app.eastus.cloudapp.azure.com)

Speculo is a workout tracking application built on a distributed microservices architecture. It demonstrates scalable system design utilizing event-driven communication, the CQRS pattern, and containerized deployment.

## Architecture

The system is divided into three core microservices, communicating asynchronously via an event bus.

*   **Identity Service**: Manages user authentication and authorization. Issues JWT tokens and stores credentials securely in PostgreSQL.
*   **Tracking Service**: Handles the core domain logic for logging and managing workouts. It serves as the authoritative source for workout data (the command side of CQRS), persisting state to PostgreSQL and publishing domain events to the messaging broker.
*   **Analytics Service**: Consumes events from the messaging broker to generate materialized views of user statistics. It serves as the read-optimized layer (the query side of CQRS), storing denormalized data in MongoDB. It utilizes Redis for caching frequently accessed dashboard queries with event-driven invalidation.

## Technical Stack

*   **Backend**: .NET 8 (C#), ASP.NET Core Web API
*   **Frontend**: React (Vite), Nginx
*   **Messaging**: Apache Kafka
*   **Databases**: PostgreSQL (Relational), MongoDB (NoSQL)
*   **Caching**: Redis
*   **Infrastructure**: Docker, Docker Compose, Kubernetes, Azure Kubernetes Service (AKS)

## Local Development

The project includes a Docker Compose configuration to easily stand up the entire local environment, including all necessary databases, caching layers, and the Kafka broker.

1. Ensure Docker Desktop is running.
2. From the project root directory, build and start the infrastructure and services:
   ```bash
   docker compose up -d --build
   ```
3. The frontend application will be accessible at `http://localhost:3000`. API requests are routed through the Nginx reverse proxy.

## Deployment

The application is containerized and configured for deployment to Kubernetes. The necessary manifests for Deployments, Services, and LoadBalancers are located in the `/k8s` directory. The production setup involves pushing images to a container registry (e.g., Azure Container Registry) and applying the manifests to a cluster (e.g., Azure Kubernetes Service).
