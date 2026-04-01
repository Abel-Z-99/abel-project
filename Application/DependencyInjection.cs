using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // For now we only expose Auth services, which are wired in Infrastructure.
        return services;
    }
}
