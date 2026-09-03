using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleFileServer.Application.Abstractions.Infrastructure;
using SimpleFileServer.Infrastructure.Configs;
using SimpleFileServer.Infrastructure.EF;
using SimpleFileServer.Infrastructure.Implementations;

namespace SimpleFileServer.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        AddConfigs(services);
        AddImplementations(services);
        AddAppDbContext(services);
    }

    private static void AddImplementations(IServiceCollection services)
    {
        services.AddScoped<IDatabaseService, DatabaseService>();
        services.AddScoped<IDomainFileRepository, DomainFileRepository>();
        services.AddSingleton<IFileStore, FileStore>();
    }

    private static void AddConfigs(IServiceCollection services)
    {
        services.AddOptionsWithValidation<FileStoreOptions>("Infrastructure:FileStore");
        services.AddOptionsWithValidation<DatabaseOptions>("Infrastructure:Database");
    }

    private static void AddAppDbContext(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>((services, options) =>
        {
            IOptions<DatabaseOptions> dbOptions = services.GetRequiredService<IOptions<DatabaseOptions>>();
            options.UseSqlite(dbOptions.Value.ConnectionString);
        });
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
