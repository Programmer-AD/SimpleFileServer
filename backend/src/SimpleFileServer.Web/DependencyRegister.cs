namespace SimpleFileServer.Web;

public static class DependencyInjection
{
    public static void AddWebServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHealthChecks();

        services.AddCors(corsOptions =>
        {
            string[]? corsOrigins = config["CorsOrigins"]?
                .Split(';')?
                .Select(x => x.Trim())?
                .Where(x => !string.IsNullOrEmpty(x))?
                .ToArray();
            if (corsOrigins == null || corsOrigins.Length == 0)
            {
                // No config - no cors policy
                return;
            }

            corsOptions.AddDefaultPolicy(policyBuilder =>
            {
                policyBuilder
                    .WithOrigins(corsOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
}
