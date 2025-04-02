using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;

namespace OrderApi.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        // Add Database connectivity
        // Add Authentication scheme
        SharedServiceContainer.AddSharedService<OrderDbContext>(services, config, config["MySerilog:FileName"]!);

        // Create Dependency Injection
        services.AddScoped<IOrder, OrderRepository>();

        return services;
    }

    public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
    {
        // Register middleware such as:
        // Global exception -> handle external errors
        // ListenToOnlyApiGateway Only -> block all outsiders calls
        SharedServiceContainer.UseSharedPolicies(app);

        return app;
    }
}
