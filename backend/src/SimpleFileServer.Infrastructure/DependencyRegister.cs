using Microsoft.Extensions.DependencyInjection;
using SimpleFileServer.Application.Abstractions.Configs;
using SimpleFileServer.Application.Abstractions.Infrastructure;
using SimpleFileServer.Infrastructure.Implementations;

namespace SimpleFileServer.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        AddConfigs(services);
        AddImplementations(services);
    }

    private static void AddImplementations(IServiceCollection services)
    {
        services.AddSingleton<IFileStore, FileStore>();
        services.AddSingleton<IDomainFileRepository, DomainFileRepository>();
    }

    private static void AddConfigs(IServiceCollection services)
    {
        services.AddOptionsWithValidation<FileStoreOptions>("Infrastructure:FileStore");
    }

    private static void AddOptionsWithValidation<TOptions>(this IServiceCollection services, string sectionName)
        where TOptions : class, new()
    {
        services.AddOptions<TOptions>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

}
