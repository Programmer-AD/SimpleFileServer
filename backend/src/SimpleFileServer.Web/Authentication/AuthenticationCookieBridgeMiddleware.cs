using SimpleFileServer.Web.Utils;

namespace SimpleFileServer.Web.Authentication;

internal class AuthenticationCookieBridgeMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // If Authorization header is not provided, get it using cookie bridge (usefule for SPAs to prevent XSS)
        if (context.Request.Headers.Authorization.Count == 0
            && SplitCookieHandler.TryGetSplitCookieValue(
                context.Request.Cookies.ToDictionary(x => x.Key, x => x.Value),
                WebConstants.AuthBridgeCookieName,
                out string? authValue))
        {
            context.Request.Headers.Authorization = authValue;
        }

        await next(context);
    }
}
