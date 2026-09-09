namespace SimpleFileServer.IntegrationTests.Endpoints;

public class OpenApiEndpointTests : IClassFixture<CustomApplicationFactory>
{
    private readonly CustomApplicationFactory appFactory;

    public OpenApiEndpointTests(CustomApplicationFactory appFactory)
    {
        this.appFactory = appFactory;
    }

    [Fact]
    public async Task OpenApiEndpoint_ShouldReturnSomething()
    {
        using HttpClient httpClient = appFactory.CreateClient();

        string result = await httpClient.GetStringAsync("/openapi/v1.json");

        Assert.NotEmpty(result);
    }

    private record class MiniHealthCheckResult(string OverallStatus);
}
