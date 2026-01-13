# Speculo: Script Correlation Map �

This document maps exactly which files "talk" to each other. Click the file links to navigate through the dependency chain.

---

## 🏗️ 1. The "Log Mood" Flow (Event Sourcing)
This is the journey of a life metric being saved to the database.

*   **Entry Point**: [EventController.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.API/Controllers/EventController.cs)
    *   **Depends on**: [LogMoodCommand.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Application/Features/Events/Commands/LogMood/LogMoodCommand.cs) (The data container)
    *   **Uses**: `ISender` (MediatR) to deliver the command.
*   **The Processor**: [LogMoodCommandHandler.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Application/Features/Events/Commands/LogMood/LogMoodCommandHandler.cs)
    *   **Depends on**: [LogMoodCommand.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Application/Features/Events/Commands/LogMood/LogMoodCommand.cs) (The input)
    *   **Depends on**: [IEventStore.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Application/Common/Interfaces/IEventStore.cs) (The storage contract)
    *   **Uses**: [MoodLoggedEvent.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Domain/Events/MoodLoggedEvent.cs) to create the domain-record.
*   **The Storage Logic**: [EventStore.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Infrastructure/Services/EventStore.cs)
    *   **Implements**: [IEventStore.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Application/Common/Interfaces/IEventStore.cs)
    *   **Depends on**: [ISpeculoDbContext.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Application/Common/Interfaces/ISpeculoDbContext.cs) (To talk to DB)
    *   **Uses Entity**: [Event.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Domain/Entities/Event.cs) (The database table row)

---

## 🔐 2. The Identity & Security Flow
This is the journey of a user logging in and getting a token.

*   **Entry Point**: [AuthController.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.API/Controllers/AuthController.cs)
    *   **Depends on**: [IIdentityService.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Application/Common/Interfaces/IIdentityService.cs)
*   **The Logic**: [IdentityService.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Infrastructure/Services/IdentityService.cs)
    *   **Depends on**: [ISpeculoDbContext.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Application/Common/Interfaces/ISpeculoDbContext.cs) (To find the user)
    *   **Depends on**: [IJwtTokenGenerator.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Application/Common/Interfaces/IJwtTokenGenerator.cs) (To create the token)
    *   **Uses Entity**: [User.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Domain/Entities/User.cs)
*   **The Generator**: [JwtTokenGenerator.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Infrastructure/Authentication/JwtTokenGenerator.cs)
    *   **Implements**: [IJwtTokenGenerator.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Application/Common/Interfaces/IJwtTokenGenerator.cs)
    *   **Depends on**: [JwtSettings.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Infrastructure/Authentication/JwtSettings.cs) (Secret keys)

---

## 🗄️ 3. Persistence (Database Setup)
How the code maps to the actual tables.

*   **The Bridge**: [SpeculoDbContext.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Infrastructure/SpeculoDbContext.cs)
    *   **Implements**: [ISpeculoDbContext.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Application/Common/Interfaces/ISpeculoDbContext.cs)
    *   **Depends on**: [EventConfiguration.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Infrastructure/Configurations/EventConfiguration.cs) (Metadata rules)
    *   **Depends on**: [UserConfiguration.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Infrastructure/Configurations/UserConfiguration.cs)

---

## 📦 4. Registry (Wiring it all up)
Where we tell the app: "Use THIS script for THAT interface."

*   [Infrastructure/DependencyInjection.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Infrastructure/DependencyInjection.cs): Maps Infrastructure implementations to Application contracts.
*   [Application/DependencyInjection.cs](file:///c:/Users/Juls/Documents/GitHub/Speculo/Speculo.Application/DependencyInjection.cs): Registers the "Brain" logic (MediatR).
