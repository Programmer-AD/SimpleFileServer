using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SimpleFileServer.Domain.Exceptions;

namespace SimpleFileServer.Web.ExceptionHandling;

internal class CustomExceptionHandler(
    IProblemDetailsWriter problemDetailsWriter,
    ILogger<CustomExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails = GetProblemDetails(exception);

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        await problemDetailsWriter.WriteAsync(new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails,
        });

        if (problemDetails.Status == 500)
        {
            logger.LogError(exception, "An error have occured.");
        }

        return true;
    }

    private static ProblemDetails GetProblemDetails(Exception exception)
    {
        var domainException = exception as DomainException;

        return new ProblemDetails()
        {
            Status = GetResultStatusCode(domainException),
            Type = domainException?.TypeName,
            Detail = domainException?.Details ?? "Internal server error occured",
        };
    }

    private static int GetResultStatusCode(DomainException? domainException)
    {
        if (string.IsNullOrEmpty(domainException?.TypeName))
        {
            return 500;
        }

        if (domainException.TypeName.StartsWith(DomainExceptionTypes.GenericNotFound))
        {
            return 404;
        }

        if (domainException.TypeName.StartsWith(DomainExceptionTypes.GenericAccessDenied))
        {
            return 403;
        }

        return 500;
    }
}
