using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Speculo.Application.Common.Interfaces;
using Speculo.Infrastructure.Services;
using Speculo.Infrastructure.Authentication;

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

        // Identity Services
        services.AddScoped<IIdentityService, IdentityService>();
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        // Event Sourcing
        services.AddScoped<IEventStore, EventStore>();

        // User Identity Context
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        return services;
    }
}