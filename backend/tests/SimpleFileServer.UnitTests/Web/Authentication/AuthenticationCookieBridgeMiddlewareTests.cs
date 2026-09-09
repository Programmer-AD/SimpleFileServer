using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SimpleFileServer.Web;
using SimpleFileServer.Web.Authentication;
using SimpleFileServer.Web.Utils;

namespace SimpleFileServer.UnitTests.Web.Authentication;

public class AuthenticationCookieBridgeMiddlewareTests
{
    private const string InitialAuthHeader = "Bearer InitialToken";
    private const string AuthHeaderFromCookies = "Bearer TokenFromCookie";

    private readonly HttpContext httpContext;

    private readonly AuthenticationCookieBridgeMiddleware bridgeMiddleware;

    public AuthenticationCookieBridgeMiddlewareTests()
    {
        httpContext = new DefaultHttpContext();

        bridgeMiddleware = new();
    }

    [Fact]
    public async Task InvokeAsync_WhenThereIsAuthHeader_DoesntChangeAuthHeader()
    {
        SetupHttpContext(InitialAuthHeader, GetAuthCookies());

        await bridgeMiddleware.InvokeAsync(httpContext, _ => Task.CompletedTask);

        Assert.Equal(InitialAuthHeader, httpContext.Request.Headers.Authorization);
    }

    [Fact]
    public async Task InvokeAsync_WhenCannotGetAuthHeaderFromCookies_DoesntChangeAuthHeader()
    {
        SetupHttpContext(StringValues.Empty, StringValues.Empty);

        await bridgeMiddleware.InvokeAsync(httpContext, _ => Task.CompletedTask);

        Assert.Equal(StringValues.Empty, httpContext.Request.Headers.Authorization);
    }

    [Fact]
    public async Task InvokeAsync_WhenGotAuthHeaderFromCookies_ChangesAuthHeader()
    {
        SetupHttpContext(StringValues.Empty, GetAuthCookies());

        await bridgeMiddleware.InvokeAsync(httpContext, _ => Task.CompletedTask);

        Assert.Equal(AuthHeaderFromCookies, httpContext.Request.Headers.Authorization);
    }

    private void SetupHttpContext(StringValues authHeader, StringValues cookies)
    {
        httpContext.Request.Headers.Authorization = authHeader;
        httpContext.Request.Headers.Cookie = cookies;
    }

    private static StringValues GetAuthCookies()
        => new([.. SplitCookieHandler.ToSplitCookies(WebConstants.AuthBridgeCookieName, AuthHeaderFromCookies).Select(x => $"{x.Key}={x.Value}")]);
}
