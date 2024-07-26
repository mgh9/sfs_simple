using Microsoft.Extensions.DependencyInjection;
using SFS.Application.Abstractions.Services;
using SFS.Application.Services;

namespace SFS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
