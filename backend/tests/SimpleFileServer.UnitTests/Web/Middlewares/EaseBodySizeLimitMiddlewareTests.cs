using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using SimpleFileServer.Web.Middlewares;

namespace SimpleFileServer.UnitTests.Web.Middlewares;

public class EaseBodySizeLimitMiddlewareTests
{
    private readonly EaseBodySizeLimitMiddlewareOptions options = new()
    {
        DesiredSizeLimit = 123456578,
    };

    private readonly Mock<IHttpMaxRequestBodySizeFeature> bodySizeFeatureMock;
    private readonly HttpContext httpContext;

    private readonly EaseBodySizeLimitMiddleware middleware;

    public EaseBodySizeLimitMiddlewareTests()
    {
        bodySizeFeatureMock = new();
        bodySizeFeatureMock.SetupProperty(x => x.MaxRequestBodySize);

        httpContext = new DefaultHttpContext();
        httpContext.Features.Set(bodySizeFeatureMock.Object);

        middleware = new(Options.Create(options));
    }

    [Theory]
    [InlineData("/api/files/6652ea55-a762-4f2f-82d7-98a047404aa8/content")]
    [InlineData("/api/files/c5cd5a2c-db4d-4e74-bbd6-21449db95763/content")]
    [InlineData("/api/files/also-matches/content")]
    public async Task InvokeAsync_WhenShouldBeEased_ChangesBodySizeLimit(string path)
    {
        httpContext.Request.Path = path;

        await middleware.InvokeAsync(httpContext, _ => Task.CompletedTask);

        Assert.Equal(options.DesiredSizeLimit, bodySizeFeatureMock.Object.MaxRequestBodySize);
    }

    [Theory]
    [InlineData("/api/test")]
    [InlineData("/api/files//content")]
    [InlineData("/api/files/6652ea55-a762-4f2f-82d7-98a047404aa8/content-other")]
    [InlineData("/api/files/subfile/c5cd5a2c-db4d-4e74-bbd6-21449db95763/content")]
    [InlineData("/api/files/c5cd5a2c-db4d-4e74-bbd6-21449db95763/something-more/content")]
    public async Task InvokeAsync_WhenShouldNotBeEased_DoesNotChangeBodySizeLimit(string path)
    {
        httpContext.Request.Path = path;

        await middleware.InvokeAsync(httpContext, _ => Task.CompletedTask);

        Assert.Null(bodySizeFeatureMock.Object.MaxRequestBodySize);
    }

    [Fact]
    public async Task InvokeAsync_WhenFeatureIsNotPresent_DoesNotCrash()
    {
        httpContext.Request.Path = "/api/files/6652ea55-a762-4f2f-82d7-98a047404aa8/content";
        httpContext.Features.Set<IHttpMaxRequestBodySizeFeature>(null);

        await middleware.InvokeAsync(httpContext, _ => Task.CompletedTask);
    }
}
