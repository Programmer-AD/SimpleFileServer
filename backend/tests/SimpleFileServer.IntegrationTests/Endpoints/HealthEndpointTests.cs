namespace SimpleFileServer.IntegrationTests.Endpoints;

public class HealthEndpointTests : IClassFixture<CustomApplicationFactory>
{
    private readonly CustomApplicationFactory appFactory;

    public HealthEndpointTests(CustomApplicationFactory appFactory)
    {
        this.appFactory = appFactory;
    }

    [Fact]
    public async Task HealthEndpoint_ShouldReportHealthyState()
    {
        using HttpClient httpClient = appFactory.CreateClient();

        MiniHealthCheckResult? result = await httpClient.GetFromJsonAsync<MiniHealthCheckResult>("/health");

        Assert.Equal("Healthy", result?.OverallStatus);
    }

    private record class MiniHealthCheckResult(string OverallStatus);
}
