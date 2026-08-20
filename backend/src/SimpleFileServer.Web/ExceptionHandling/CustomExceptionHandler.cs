using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SimpleFileServer.Web.ExceptionHandling;

internal class CustomExceptionHandler(
    IProblemDetailsWriter problemDetailsWriter,
    ILogger<CustomExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        await problemDetailsWriter.WriteAsync(new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = GetProblemDetails(exception),
        });

        logger.LogError(exception, "An error have occured.");

        return true;
    }

    private static ProblemDetails GetProblemDetails(Exception exception)
    {
        // Add custom exception handling logic as needed

        return new ProblemDetails()
        {
            Status = 500,
            Title = "An error occurred",
            Detail = exception.Message,
        };
    }
}
