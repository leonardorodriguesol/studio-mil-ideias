using Microsoft.Extensions.DependencyInjection;
using StudioMilIdeias.Application.UseCases.Auth;
using StudioMilIdeias.Application.UseCases.Products;

namespace StudioMilIdeias.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductQueries, ProductQueries>();
        return services;
    }
}
