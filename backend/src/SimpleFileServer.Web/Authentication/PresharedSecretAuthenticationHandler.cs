using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace SimpleFileServer.Web.Authentication;

/// <summary>
///     Pretty weak authentication, yet better than nothing.
///     Mostly present as example of custom auth implementation.
/// </summary>
internal class PresharedSecretAuthenticationHandler : AuthenticationHandler<PresharedSecretAuthenticationHandlerOptions>
{
    public const string AuthenticationScheme = "PresharedSecret";

    public PresharedSecretAuthenticationHandler(
        IOptionsMonitor<PresharedSecretAuthenticationHandlerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
    : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        => Task.FromResult(HandleAuthenticate());

    private AuthenticateResult HandleAuthenticate()
    {

        if (AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out AuthenticationHeaderValue? authenticationHeader)
            || authenticationHeader == null)
        {
            return AuthenticateResult.Fail("Authentication header is not present or has incorrect format.");
        }

        if (!authenticationHeader.Scheme.Equals(AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.Fail($"Authentication scheme \"{authenticationHeader.Scheme}\" is incorrect. Expected value \"{AuthenticationScheme}\" (case insensitive).");
        }

        if (authenticationHeader.Parameter != Options.PresharedSecret)
        {
            return AuthenticateResult.Fail("Provided secret is incorrect");
        }

        return AuthenticateResult.Success(CreateAuthenticationTicket([
            new Claim(ClaimTypes.NameIdentifier, "user-id-mock"),
            new Claim(ClaimTypes.Name, "User"),
        ]));
    }

    private static AuthenticationTicket CreateAuthenticationTicket(IEnumerable<Claim> claims)
        => new(new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationScheme)), AuthenticationScheme);
}

internal class PresharedSecretAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
    public string PresharedSecret { get; set; } = string.Empty;
}
