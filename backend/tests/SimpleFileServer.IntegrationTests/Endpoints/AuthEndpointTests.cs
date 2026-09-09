using System.Net;
using SimpleFileServer.IntegrationTests.Clients;

namespace SimpleFileServer.IntegrationTests.Endpoints;

public sealed class AuthEndpointTests : IClassFixture<CustomApplicationFactory>
{
    private readonly CustomApplicationFactory appFactory;

    public AuthEndpointTests(CustomApplicationFactory appFactory)
    {
        this.appFactory = appFactory;
    }

    [Fact]
    public async Task GetAuthCookiesAsync_ReturnsCookiesSuitableForFurtherAuth()
    {
        using HttpClient httpClient = appFactory.CreateClient(new()
        {
            HandleCookies = true,
        });
        var authClient = new AuthClient(httpClient);

        // Cookies get attached to httpClient
        (await authClient.GetAuthCookiesAsync()).EnsureSuccessStatusCode();

        // This endpoint requires authroization, so call again using cookies to test bridge
        httpClient.DefaultRequestHeaders.Authorization = null;
        HttpResponseMessage response = await authClient.GetAuthCookiesAsync();

        Assert.True(response.IsSuccessStatusCode, $"Status code {response.StatusCode} is not successfull");
    }

    [Fact]
    public async Task DeleteAuthCookiesAsync_RemoveAuthCookies()
    {
        using HttpClient httpClient = appFactory.CreateClient(new()
        {
            HandleCookies = true,
        });
        var authClient = new AuthClient(httpClient);

        // Cookies get attached to httpClient
        (await authClient.GetAuthCookiesAsync()).EnsureSuccessStatusCode();

        // Stop sending header and remove auth cookies
        httpClient.DefaultRequestHeaders.Authorization = null;
        (await authClient.DeleteAuthCookiesAsync()).EnsureSuccessStatusCode();

        // This endpoint requires authorization, so call without cookies to validate that cookies are removed
        HttpResponseMessage response = await authClient.GetAuthCookiesAsync();
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAuthCookiesAsync_AllowsAnonymousCalls()
    {
        using HttpClient httpClient = appFactory.CreateClient(new()
        {
            HandleCookies = true,
        });
        var authClient = new AuthClient(httpClient);

        httpClient.DefaultRequestHeaders.Authorization = null;

        HttpResponseMessage response = await authClient.DeleteAuthCookiesAsync();

        Assert.True(response.IsSuccessStatusCode, $"Status code {response.StatusCode} is not successfull");
    }
}
