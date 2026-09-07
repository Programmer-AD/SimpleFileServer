using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using NLog;
using NLog.Web;
using SimpleFileServer.Application;
using SimpleFileServer.Application.Abstractions.Infrastructure;
using SimpleFileServer.Infrastructure;
using SimpleFileServer.Web.Authentication;
using SimpleFileServer.Web.Endpoints;
using SimpleFileServer.Web.HealthChecks;

namespace SimpleFileServer.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        try
        {
            logger.Info("Application is starting...");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            SetupServices(builder);

            WebApplication app = builder.Build();
            SetupApp(app);

            await InitAppAsync(app);

            await app.RunAsync();
        }
#if DEBUG
        catch (HostAbortedException)
        {
            // This exception can occur when manipulating EF Core migrations
            // So we suppress it's logging on debug
        }
#endif
        catch (Exception exception)
        {
            logger.Fatal(exception, "Critical exception has occured on service startup or runtime.");
        }
        finally
        {
            logger.Info("Application is shutting down...");
            LogManager.Shutdown();
        }
    }

    private static void SetupServices(WebApplicationBuilder builder)
    {
        // Setup NLog
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        IConfiguration config = builder.Configuration;

        IServiceCollection services = builder.Services;
        services.AddInfrastructureServices();
        services.AddApplicationServices();
        services.AddWebServices(config);
    }

    private static void SetupApp(WebApplication app)
    {
        app.UseExceptionHandler();
        // To display problem details for empty unsuccessful responses
        app.UseStatusCodePages();

        app.UseRouting();

        // Must go before authentication
        app.UseMiddleware<AuthenticationCookieBridgeMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.MapStaticAssets().ShortCircuit();

        app.MapHealthChecks("/health", new HealthCheckOptions()
        {
            ResponseWriter = HealthCheckResultWriter.WriteResultAsync,
        });

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "SimpleFileServer"));
        }

        app.MapAppEndpoints();
    }

    private static async Task InitAppAsync(WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IDatabaseService dbService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
        await dbService.EnsureInitializedAsync();
    }
}
