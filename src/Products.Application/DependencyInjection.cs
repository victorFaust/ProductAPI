using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Products.Application.Services;
using AutoMapper;
using Products.Application.Common.MappingProfiles;

namespace Products.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ProductService>();
        services.AddAutoMapper(typeof(ProductProfile));
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
