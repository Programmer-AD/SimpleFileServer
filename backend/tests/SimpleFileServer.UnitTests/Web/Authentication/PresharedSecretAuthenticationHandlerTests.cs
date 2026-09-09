using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using SimpleFileServer.Web.Authentication;

namespace SimpleFileServer.UnitTests.Web.Authentication;

public class PresharedSecretAuthenticationHandlerTests
{
    private const string PresharedSecret = "TestSecret";

    private readonly HttpContext httpContext;
    private readonly PresharedSecretAuthenticationHandlerOptions options;

    private readonly PresharedSecretAuthenticationHandler authenticationHandler;

    public PresharedSecretAuthenticationHandlerTests()
    {
        httpContext = new DefaultHttpContext();

        options = new()
        {
            PresharedSecret = PresharedSecret,
        };
        var optionsMock = new Mock<IOptionsMonitor<PresharedSecretAuthenticationHandlerOptions>>();
        optionsMock.Setup(x => x.Get(It.IsAny<string>())).Returns(options);

        authenticationHandler = new PresharedSecretAuthenticationHandler(optionsMock.Object, new LoggerFactory(), UrlEncoder.Default);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenAuthHeaderIsNotPresent_ReturnsFailedResult()
    {
        SetAuthHeader(StringValues.Empty);
        await InitializeAuthHandler();

        AuthenticateResult result = await authenticationHandler.AuthenticateAsync();

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenProvidedAuthenticationSchemeIsNotCorrect_ReturnsFailedResult()
    {
        SetAuthHeader($"OtherScheme {PresharedSecret}");
        await InitializeAuthHandler();

        AuthenticateResult result = await authenticationHandler.AuthenticateAsync();

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenProvidedSecretIsNotCorrect_ReturnsFailedResult()
    {
        SetAuthHeader($"{PresharedSecretAuthenticationHandler.AuthenticationScheme} WrongSecret");
        await InitializeAuthHandler();

        AuthenticateResult result = await authenticationHandler.AuthenticateAsync();

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenEverythingIsFine_ReturnsSuccessResult()
    {
        SetAuthHeader($"{PresharedSecretAuthenticationHandler.AuthenticationScheme} {PresharedSecret}");

        await InitializeAuthHandler();

        AuthenticateResult result = await authenticationHandler.AuthenticateAsync();

        Assert.True(result.Succeeded);
    }

    private Task InitializeAuthHandler()
        => authenticationHandler.InitializeAsync(
            new(PresharedSecretAuthenticationHandler.AuthenticationScheme, null, typeof(PresharedSecretAuthenticationHandler)), httpContext);

    private void SetAuthHeader(StringValues authHeader)
    {
        httpContext.Request.Headers.Authorization = authHeader;
    }
}
