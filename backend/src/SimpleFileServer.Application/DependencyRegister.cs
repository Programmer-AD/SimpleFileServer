using Microsoft.Extensions.DependencyInjection;
using SimpleFileServer.Application.Abstractions.Services;
using SimpleFileServer.Application.Services;

namespace SimpleFileServer.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        AddServices(services);
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IDomainFileService, DomainFileService>();
    }
}
