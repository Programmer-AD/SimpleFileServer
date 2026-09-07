using SimpleFileServer.Web.Authentication;
using SimpleFileServer.Web.ExceptionHandling;
using SimpleFileServer.Web.HealthChecks;

namespace SimpleFileServer.Web;

public static class DependencyInjection
{
    public static void AddWebServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddValidation();

        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("Database");

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                // For log based diagnostics
                context.ProblemDetails.Extensions["requestId"] = context.HttpContext.TraceIdentifier;
                context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow.ToString("O");

                // Remove long but useless extension (it is not part of logs)
                context.ProblemDetails.Extensions.Remove("traceId");
            };
        });
        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddOpenApi();

        services.AddAuthentication()
            .AddScheme<PresharedSecretAuthenticationHandlerOptions, PresharedSecretAuthenticationHandler>(
                PresharedSecretAuthenticationHandler.AuthenticationScheme,
                options =>
                {
                    string? presharedSecret = config["Authentication:PresharedSecret"];
                    if (string.IsNullOrEmpty(presharedSecret))
                    {
                        throw new ApplicationException($"Preshared secret is empty or missing. Please configure it.");
                    }

                    options.PresharedSecret = presharedSecret;
                });

        services.AddAuthorization();
    }
}
