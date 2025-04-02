using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        // Add Database connectivity
        // Add Authentication scheme
        SharedServiceContainer.AddSharedService<AuthenticationDbContext>(services, config, config["MySerilog:FileName"]);


        // Add Dependency Injection
        services.AddScoped<IUser, UserRepository>();

        return services;
    }

    public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
    {
        // Register services such as:
        // Global Exception : Handle Global Errors
        // ListenToOnlyApiGateway Only : block all outsiders call
        SharedServiceContainer.UseSharedPolicies(app);

        return app;
    }
}
