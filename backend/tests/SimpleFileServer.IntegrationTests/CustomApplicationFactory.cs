using Microsoft.AspNetCore.Mvc.Testing;
using SimpleFileServer.Web;

namespace SimpleFileServer.IntegrationTests;

public sealed class CustomApplicationFactory : WebApplicationFactory<Program>
{
    private const string TestPresharedSecret = "TestSecret";

    private readonly string testRunId;
    private readonly string testRunDataLocation;

    public CustomApplicationFactory()
    {
        testRunId = $"{DateTime.UtcNow:yyMMdd_HHmmss}_{Random.Shared.Next(0, ushort.MaxValue):X4}";
        testRunDataLocation = $"./test/{testRunId}";
    }

    protected override void ConfigureWebHost(IWebHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((context, services) =>
        {
            context.Configuration["Infrastructure:Database:ConnectionString"] = $"Data Source={testRunDataLocation}/data.db";
            context.Configuration["Infrastructure:FileStore:StorageFolderPath"] = $"{testRunDataLocation}/storage";
            context.Configuration["Authentication:PresharedSecret"] = TestPresharedSecret;
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = new("PresharedSecret", TestPresharedSecret);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        // Make sure SQLite is closed, so we can drop its file
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();

        if (Directory.Exists(testRunDataLocation))
        {
            Directory.Delete(testRunDataLocation, recursive: true);
        }
    }
}
