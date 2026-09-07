using System.Net.Mime;

namespace SimpleFileServer.Web.Endpoints;

internal static class AppEndpointRegister
{
    public static void MapAppEndpoints(this WebApplication app)
    {
        // If antiforgery is not disabled - Swagger UI fails on file uploads
        RouteGroupBuilder rootGroup = app.MapGroup("api")
            .DisableAntiforgery()
            .RequireAuthorization()
            .ProducesProblem(401);

        MapFileEndpoints(rootGroup);

        RouteGroupBuilder noAuthGroup = app.MapGroup("api")
            .AllowAnonymous();
        MapAuthEndpoints(noAuthGroup);
    }

    private static void MapFileEndpoints(IEndpointRouteBuilder routeBuilder)
    {
        RouteGroupBuilder fileGroup = routeBuilder.MapGroup("files");
        fileGroup.MapPost("", FileEndpoints.UploadAsync);
        fileGroup.MapGet("", FileEndpoints.GetAllAsync);

        RouteGroupBuilder fileItemGroup = fileGroup.MapGroup("{id}");
        fileItemGroup.MapGet("content", FileEndpoints.GetContentAsync)
            .Produces<Stream>(200, contentType: MediaTypeNames.Application.Octet)
            .ProducesProblem(404);
        fileItemGroup.MapPatch("rename", FileEndpoints.RenameAsync)
            .ProducesProblem(404);
        fileItemGroup.MapDelete("", FileEndpoints.DeleteAsync);
    }

    private static void MapAuthEndpoints(IEndpointRouteBuilder routeBuilder)
    {
        RouteGroupBuilder authGroup = routeBuilder.MapGroup("auth");
        authGroup.MapGet("cookies", (Delegate)AuthEndpoints.GetAuthCookiesAsync)
            .RequireAuthorization()
            .ProducesProblem(401);

        authGroup.MapDelete("cookies", (Delegate)AuthEndpoints.DeleteAuthCookiesAsync)
            .AllowAnonymous();
    }
}
