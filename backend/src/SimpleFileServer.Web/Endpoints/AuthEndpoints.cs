using Microsoft.AspNetCore.Http.HttpResults;
using SimpleFileServer.Web.Utils;

namespace SimpleFileServer.Web.Endpoints;

public static class AuthEndpoints
{
    /// <summary>
    ///     Puts current authentication header value to HttpOnly cookies.
    ///     This endpoint is used by frontend to prevent XSS stealing of data.
    /// </summary>
    public static async Task<Ok> GetAuthCookiesAsync(HttpContext httpContext)
    {
        Dictionary<string, string> authCookies = SplitCookieHandler.ToSplitCookies(WebConstants.AuthBridgeCookieName, httpContext.Request.Headers.Authorization.ToString());

        foreach ((string key, string value) in authCookies)
        {
            httpContext.Response.Cookies.Append(key, value, new CookieOptions()
            {
                // The key line, make auth data inaccessible to JS
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
            });
        }

        return TypedResults.Ok();
    }
}
