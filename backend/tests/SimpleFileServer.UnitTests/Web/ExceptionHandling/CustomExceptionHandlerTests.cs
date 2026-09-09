using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleFileServer.Domain.Exceptions;
using SimpleFileServer.Web.ExceptionHandling;

namespace SimpleFileServer.UnitTests.Web.ExceptionHandling;

public class CustomExceptionHandlerTests
{
    private readonly Mock<IProblemDetailsWriter> problemDetailsWriterMock;
    private readonly Mock<ILogger<CustomExceptionHandler>> loggerMock;
    private readonly HttpContext httpContext;

    private readonly CustomExceptionHandler customExceptionHandler;

    public CustomExceptionHandlerTests()
    {
        problemDetailsWriterMock = new();
        loggerMock = new();
        httpContext = new DefaultHttpContext();

        customExceptionHandler = new CustomExceptionHandler(
            problemDetailsWriterMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task TryHandleAsync_WhenExceptionIsNotFromDomain_WritesProblem500()
    {
        var exception = new NullReferenceException("Test null reference exception.");
        ProblemDetailsContext expectedProblem = GetProblemDetailsContext(exception, new ProblemDetails
        {
            Status = 500,
            Type = null,
            Detail = "Internal server error occured",
        });

        ProblemDetailsContext? actualProblem = null;
        problemDetailsWriterMock
            .Setup(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback((ProblemDetailsContext problem) => actualProblem = problem);

        await customExceptionHandler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.Equivalent(expectedProblem, actualProblem);
    }

    [Fact]
    public async Task TryHandleAsync_WhenExceptionIsNotFromDomain_WritesErrorLog()
    {
        var exception = new NullReferenceException("Test null reference exception.");

        await customExceptionHandler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // This log is critical for application troubleshooting, so we test that we do not forget about it
        loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), exception, It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }

    [Fact]
    public async Task TryHandleAsync_WhenDomainExceptionTypeIsGenericNotFound_WritesProblem404()
    {
        const string domainExceptionType = DomainExceptionTypes.GenericNotFound;
        const string domainExceptionDetails = "Test details.";
        var exception = new DomainException(domainExceptionType, domainExceptionDetails);
        ProblemDetailsContext expectedProblem = GetProblemDetailsContext(exception, new ProblemDetails
        {
            Status = 404,
            Type = domainExceptionType,
            Detail = domainExceptionDetails,
        });

        ProblemDetailsContext? actualProblem = null;
        problemDetailsWriterMock
            .Setup(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback((ProblemDetailsContext problem) => actualProblem = problem);

        await customExceptionHandler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.Equivalent(expectedProblem, actualProblem);
    }

    [Fact]
    public async Task TryHandleAsync_WhenDomainExceptionTypeIsNotFoundSubType_WritesProblem404()
    {
        const string domainExceptionType = $"{DomainExceptionTypes.GenericNotFound}/subType";
        const string domainExceptionDetails = "Test details.";
        var exception = new DomainException(domainExceptionType, domainExceptionDetails);
        ProblemDetailsContext expectedProblem = GetProblemDetailsContext(exception, new ProblemDetails
        {
            Status = 404,
            Type = domainExceptionType,
            Detail = domainExceptionDetails,
        });

        ProblemDetailsContext? actualProblem = null;
        problemDetailsWriterMock
            .Setup(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback((ProblemDetailsContext problem) => actualProblem = problem);

        await customExceptionHandler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.Equivalent(expectedProblem, actualProblem);
    }

    [Fact]
    public async Task TryHandleAsync_WhenDomainExceptionTypeIsGenericAccessDenied_WritesProblem403()
    {
        const string domainExceptionType = DomainExceptionTypes.GenericAccessDenied;
        const string domainExceptionDetails = "Test details.";
        var exception = new DomainException(domainExceptionType, domainExceptionDetails);
        ProblemDetailsContext expectedProblem = GetProblemDetailsContext(exception, new ProblemDetails
        {
            Status = 403,
            Type = domainExceptionType,
            Detail = domainExceptionDetails,
        });

        ProblemDetailsContext? actualProblem = null;
        problemDetailsWriterMock
            .Setup(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback((ProblemDetailsContext problem) => actualProblem = problem);

        await customExceptionHandler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.Equivalent(expectedProblem, actualProblem);
    }

    [Fact]
    public async Task TryHandleAsync_WhenDomainExceptionTypeIsAccessDeniedSubType_WritesProblem403()
    {
        const string domainExceptionType = $"{DomainExceptionTypes.GenericAccessDenied}/subType";
        const string domainExceptionDetails = "Test details.";
        var exception = new DomainException(domainExceptionType, domainExceptionDetails);
        ProblemDetailsContext expectedProblem = GetProblemDetailsContext(exception, new ProblemDetails
        {
            Status = 403,
            Type = domainExceptionType,
            Detail = domainExceptionDetails,
        });

        ProblemDetailsContext? actualProblem = null;
        problemDetailsWriterMock
            .Setup(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback((ProblemDetailsContext problem) => actualProblem = problem);

        await customExceptionHandler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.Equivalent(expectedProblem, actualProblem);
    }

    [Fact]
    public async Task TryHandleAsync_WhenDomainExceptionTypeIsUnknown_WritesProblem500()
    {
        const string domainExceptionType = "unknownType";
        const string domainExceptionDetails = "Test details.";
        var exception = new DomainException(domainExceptionType, domainExceptionDetails);
        ProblemDetailsContext expectedProblem = GetProblemDetailsContext(exception, new ProblemDetails
        {
            Status = 500,
            Type = domainExceptionType,
            Detail = domainExceptionDetails,
        });

        ProblemDetailsContext? actualProblem = null;
        problemDetailsWriterMock
            .Setup(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback((ProblemDetailsContext problem) => actualProblem = problem);

        await customExceptionHandler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.Equivalent(expectedProblem, actualProblem);
    }

    [Fact]
    public async Task TryHandleAsync_UsesProblemStatusCodeForResponse()
    {
        var exception = new DomainException(DomainExceptionTypes.GenericAccessDenied, "Test details.");
        const int expectedStatusCode = 403;

        await customExceptionHandler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.Equal(expectedStatusCode, httpContext.Response.StatusCode);
    }

    private ProblemDetailsContext GetProblemDetailsContext(Exception exception, ProblemDetails problemDetails)
        => new()
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails,
        };
}
