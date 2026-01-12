using Microsoft.Extensions.DependencyInjection;

namespace Speculo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Ovde ćemo dodavati npr. FluentValidation, MediatR i slično
        return services;
    }
}
