using System.Net.Mime;

namespace SimpleFileServer.Web.Endpoints;

internal static class AppEndpointRegister
{
    public static void MapAppEndpoints(this WebApplication app)
    {
        RouteGroupBuilder rootGroup = app.MapGroup("api");
        // No need for antiforgery since we do not use cookie auth
        // If it is not disabled - Swagger UI fails on file uploads
        rootGroup
            .DisableAntiforgery()
            .RequireAuthorization();

        MapFileEndpoints(rootGroup);
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
}
