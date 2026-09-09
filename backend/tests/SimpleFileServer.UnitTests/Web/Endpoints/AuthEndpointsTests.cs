using Microsoft.AspNetCore.Http;
using SimpleFileServer.Web;
using SimpleFileServer.Web.Endpoints;

namespace SimpleFileServer.UnitTests.Web.Endpoints;

public class AuthEndpointsTests
{
    private readonly HttpContext httpContext;

    public AuthEndpointsTests()
    {
        httpContext = new DefaultHttpContext();
    }

    [Fact]
    public async Task GetAuthCookiesAsync_ShouldPutAuthHeaderToCookies()
    {
        httpContext.Request.Headers.Authorization = "Test AuthHeader";

        await AuthEndpoints.GetAuthCookiesAsync(httpContext);

        // Simplified check since there are too many fields in Set-Cookie header
        string setCookieHeaderString = httpContext.Response.Headers.SetCookie.ToString();
        Assert.Multiple(
            () => Assert.Contains($"{WebConstants.AuthBridgeCookieName}_length=1", setCookieHeaderString),
            () => Assert.Contains($"{WebConstants.AuthBridgeCookieName}_0=", setCookieHeaderString)
        );
    }

    [Fact]
    public async Task DeleteAuthCookiesAsync_ShouldRemoveAuthBridgeCookies()
    {
        httpContext.Request.Headers.Cookie = new([
            $"{WebConstants.AuthBridgeCookieName}_length=1",
            $"{WebConstants.AuthBridgeCookieName}_0=value",
            $"{WebConstants.AuthBridgeCookieName}_1=dangling",
            "must_stay=testValue"
        ]);

        await AuthEndpoints.DeleteAuthCookiesAsync(httpContext);

        // Simplified check since there are too many fields in Set-Cookie header
        string setCookieHeaderString = httpContext.Response.Headers.SetCookie.ToString();
        Assert.Multiple(
            () => Assert.Contains($"{WebConstants.AuthBridgeCookieName}_length=", setCookieHeaderString),
            () => Assert.Contains($"{WebConstants.AuthBridgeCookieName}_0=", setCookieHeaderString),
            () => Assert.Contains($"{WebConstants.AuthBridgeCookieName}_1=", setCookieHeaderString),
            () => Assert.DoesNotContain("must_stay=", setCookieHeaderString)
        );
    }
}
