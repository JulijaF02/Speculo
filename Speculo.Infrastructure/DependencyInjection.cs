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
        // 1. Registracija baze - premešteno iz Program.cs
        services.AddDbContext<SpeculoDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // 2. Povezujemo interfejs sa konkretnim DbContext-om
        // Ovo omogućava da Application sloj "vidi" bazu preko ISpeculoDbContext
        services.AddScoped<ISpeculoDbContext>(provider => provider.GetRequiredService<SpeculoDbContext>());

        // 3. Registracija Identity servisa
        services.AddScoped<IIdentityService, IdentityService>();

        // 4. Registracija JWT podešavanja (Options Pattern)
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // 5. Registracija JWT Token Generator-a
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        //6. Registering EventStore 
        services.AddScoped<IEventStore, EventStore>();
        //7. Registering Current user provider
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        return services;
    }
}