namespace SimpleFileServer.Web;

public static class DependencyInjection
{
    public static void AddWebServices(this IServiceCollection services)
    {
        services.AddHealthChecks();
    }
}
