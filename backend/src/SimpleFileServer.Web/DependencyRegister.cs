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
            .AddScheme<SubnetBasedAuthenticationHandlerOptions, SubnetBasedAuthenticationHandler>(
                SubnetBasedAuthenticationHandler.AuthenticationScheme,
                options =>
                {
                    string[]? rawAllowedSubnets = config.GetSection("Authentication:AllowedSubnets").Get<string[]>();
                    if (rawAllowedSubnets == null)
                    {
                        return;
                    }

                    var parsedSubnets = rawAllowedSubnets
                        .Select(rawValue => (rawValue, parsedValue: AllowedSubnet.TryParse(rawValue, out AllowedSubnet? result) ? result : null))
                        .ToList();
                    var incorrectSubnets = parsedSubnets.Where(x => x.parsedValue == null).Select(x => x.rawValue).ToList();
                    if (incorrectSubnets.Count > 0)
                    {
                        throw new ApplicationException($"Some allowed subnets have incorrect format. {string.Join(',', incorrectSubnets.Select(x => $"\"{x}\""))}");
                    }

                    options.AllowedSubnets.AddRange(parsedSubnets.Select(x => x.parsedValue!));
                });

        services.AddAuthorization();
    }
}
