using Microsoft.Extensions.Diagnostics.HealthChecks;
using SimpleFileServer.Application.Abstractions.Infrastructure;

namespace SimpleFileServer.Web.HealthChecks;

internal class DatabaseHealthCheck(
    IDatabaseService databaseService
) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        bool canConnectToDatabase = await databaseService.IsHealthyAsync(cancellationToken);

        return canConnectToDatabase
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Unhealthy: can't connect to database.");
    }
}
