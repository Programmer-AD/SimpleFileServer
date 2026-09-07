using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace SimpleFileServer.Web.Authentication;

internal class SubnetBasedAuthenticationHandler : AuthenticationHandler<SubnetBasedAuthenticationHandlerOptions>
{
    public const string AuthenticationScheme = "IPBased";

    public SubnetBasedAuthenticationHandler(
        IOptionsMonitor<SubnetBasedAuthenticationHandlerOptions> options,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder)
        : base(options, loggerFactory, encoder)
    {
    }

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        IPAddress? ipAddress = Context.Connection.RemoteIpAddress;
        if (ipAddress == null)
        {
            return AuthenticateResult.Fail("IP Address is not available.");
        }

        if (ipAddress.AddressFamily is not AddressFamily.InterNetwork and not AddressFamily.InterNetworkV6)
        {
            return AuthenticateResult.Fail($"IP Address family \"{ipAddress.AddressFamily}\" is not supported.");
        }

        // IPv6 is 16 bytes, IPv4 - 4 bytes so would fit as well
        Span<byte> addressBytesBuffer = stackalloc byte[16];
        if (!ipAddress.TryWriteBytes(addressBytesBuffer, out int ipSize))
        {
            return AuthenticateResult.Fail($"IP Address \"{ipAddress}\" failed on conversion to bytes");
        }

        Span<byte> addressBytes = addressBytesBuffer[..ipSize];
        foreach (AllowedSubnet allowedSubnet in Options.AllowedSubnets)
        {
            if (allowedSubnet.DoesIncludeAddress(addressBytes))
            {
                return AuthenticateResult.Success(CreateAuthenticationTicket([
                    new Claim(ClaimTypes.NameIdentifier, $"IP:{ipAddress}"),
                    new Claim(ClaimTypes.Name, $"User from {ipAddress}"),
                ]));
            }
        }

        return AuthenticateResult.Fail($"IP Address \"{ipAddress}\" does not belong to any allowed subnet.");
    }

    private static AuthenticationTicket CreateAuthenticationTicket(IEnumerable<Claim> claims)
        => new(new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationScheme)), AuthenticationScheme);
}

internal class SubnetBasedAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
    public List<AllowedSubnet> AllowedSubnets { get; set; } = [];
}
