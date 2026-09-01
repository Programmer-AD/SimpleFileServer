using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleFileServer.Application.Abstractions.Infrastructure;
using SimpleFileServer.Infrastructure.Configs;
using SimpleFileServer.Infrastructure.EF;

namespace SimpleFileServer.Infrastructure.Implementations;

internal class DatabaseService(
    AppDbContext appDbContext,
    IOptions<DatabaseOptions> dbOptions,
    ILogger<DatabaseService> logger
) : IDatabaseService
{
    public async Task EnsureInitializedAsync()
    {
        EnsureParentFolderExists();

        await appDbContext.Database.MigrateAsync();
    }

    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken)
    {
        bool canConnect = await appDbContext.Database.CanConnectAsync(cancellationToken);
        return canConnect;
    }

    private void EnsureParentFolderExists()
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder(dbOptions.Value.ConnectionString);
        // The only exception is when source is ":memory:" which can be used in integrationt tests
        if (!connectionStringBuilder.DataSource.StartsWith(':'))
        {
            string? dbDirectory = Path.GetDirectoryName(connectionStringBuilder.DataSource);
            if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
            {
                logger.LogInformation("Parent DB directory \"{DbDirectory}\" doesn't exist, creating...", dbDirectory);
                Directory.CreateDirectory(dbDirectory);
            }
        }
    }
}
