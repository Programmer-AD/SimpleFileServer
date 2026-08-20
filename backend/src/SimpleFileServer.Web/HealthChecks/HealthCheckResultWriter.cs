using System.Net.Mime;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SimpleFileServer.Web.HealthChecks;

public static class HealthCheckResultWriter
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
    };

    public static async Task WriteResultAsync(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;

        HealthCheckOverallResult result = Map(healthReport);

        await context.Response.WriteAsJsonAsync(result, JsonSerializerOptions);
    }

    private static HealthCheckOverallResult Map(HealthReport healthReport)
    {
        IEnumerable<HealthCheckEntryResult> healthCheckResults = healthReport.Entries.Select(
            entry => new HealthCheckEntryResult(
                Name: entry.Key,
                Status: entry.Value.Status.ToString(),
                Duration: entry.Value.Duration,
                Description: entry.Value.Description ?? string.Empty));

        var result = new HealthCheckOverallResult(
            OverallStatus: healthReport.Status.ToString(),
            Duration: healthReport.TotalDuration,
            healthCheckResults);

        return result;
    }

    private readonly record struct HealthCheckOverallResult(
        string OverallStatus,
        TimeSpan Duration,
        IEnumerable<HealthCheckEntryResult> HealthCheckResults);

    private readonly record struct HealthCheckEntryResult(
        string Name,
        string Status,
        TimeSpan Duration,
        string Description);
}
