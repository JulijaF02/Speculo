using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Speculo.Application.Common.Interfaces;
using Speculo.Infrastructure.Authentication;
using Speculo.Infrastructure.Messaging;
using Speculo.Infrastructure.Services;

namespace Speculo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Registration
        services.AddDbContext<SpeculoDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Map interface to implementation for Database Context
        services.AddScoped<ISpeculoDbContext>(provider => provider.GetRequiredService<SpeculoDbContext>());

        // JWT Settings (still needed for token validation)
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // Event Sourcing
        services.AddScoped<IEventStore, EventStore>();

        // Kafka Event Bus — publishes integration events to Kafka for other services
        // Singleton because Kafka producers are thread-safe and reuse one TCP connection
        services.AddSingleton<IEventBus, KafkaEventBus>();

        // User Identity Context
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        return services;
    }
}
