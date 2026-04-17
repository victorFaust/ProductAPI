using Microsoft.Extensions.DependencyInjection;

namespace Products.API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        return services;
    }
}
