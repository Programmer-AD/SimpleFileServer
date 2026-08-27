using Microsoft.AspNetCore.Cors.Infrastructure;
using SimpleFileServer.Web.ExceptionHandling;

namespace SimpleFileServer.Web;

public static class DependencyInjection
{
    public static void AddWebServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHealthChecks();

        services.AddCors(corsOptions => SetupCors(corsOptions, config));

        services.AddProblemDetails();
        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddOpenApi();
    }

    private static void SetupCors(CorsOptions corsOptions, IConfiguration config)
    {
        string[]? corsOrigins = config["CorsOrigins"]?
            .Split(';')?
            .Select(x => x.Trim())?
            .Where(x => !string.IsNullOrEmpty(x))?
            .ToArray();

        if (corsOrigins == null || corsOrigins.Length == 0)
        {
            // No config - no cors policy
            return;
        }

        corsOptions.AddDefaultPolicy(policyBuilder =>
        {
            policyBuilder
                .WithOrigins(corsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    }
}
