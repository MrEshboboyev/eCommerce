﻿using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        // Add database connectivity
        // Add authentication scheme
        SharedServiceContainer.AddSharedService<ProductDbContext>(services, config, config["MySerilog:Filename"]!);

        // Create Dependency Injection (DI)
        services.AddScoped<IProduct, ProductRepository>();

        return services;
    }

    public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
    {
        // Register middleware such as : 
        // Global Exception: handles external errors.
        // Listen to Only Api Gateway: blocks all outsider calls;
        SharedServiceContainer.UseSharedPolicies(app);

        return app;
    }
}
