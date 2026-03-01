using Microsoft.Extensions.DependencyInjection;
using StudioMilIdeias.Application.UseCases.Auth;
using StudioMilIdeias.Application.UseCases.Cart;
using StudioMilIdeias.Application.UseCases.Checkout;
using StudioMilIdeias.Application.UseCases.Products;

namespace StudioMilIdeias.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddScoped<IProductQueries, ProductQueries>();
        services.AddScoped<IProductManagementService, ProductManagementService>();
        return services;
    }
}
