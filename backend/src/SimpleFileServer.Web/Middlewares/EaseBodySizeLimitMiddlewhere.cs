using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

namespace SimpleFileServer.Web.Middlewares;

internal partial class EaseBodySizeLimitMiddlewhere(IOptions<EaseBodySizeLimitMiddlewhereOptions> options) : IMiddleware
{
    private static readonly Regex[] EasedPathRexes = [
        GetInitFileContentPathRegex(),
    ];

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        IHttpMaxRequestBodySizeFeature bodySizeFeature = context.Features.GetRequiredFeature<IHttpMaxRequestBodySizeFeature>();

        if (ShouldBeEased(context.Request.Path))
        {
            // Attribute based approach doesn't allow us to use configs
            bodySizeFeature.MaxRequestBodySize = options.Value.DesiredSizeLimit;
        }

        await next(context);
    }

    private static bool ShouldBeEased(string path)
        => EasedPathRexes.Any(regex => regex.IsMatch(path));

    [GeneratedRegex("api/files/[^/]+/content$")]
    private static partial Regex GetInitFileContentPathRegex();
}

internal class EaseBodySizeLimitMiddlewhereOptions
{
    public long DesiredSizeLimit { get; set; }
}
