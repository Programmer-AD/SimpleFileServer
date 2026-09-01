namespace SimpleFileServer.Application.Abstractions.Infrastructure;

public interface IDatabaseService
{
    Task EnsureInitializedAsync();

    Task<bool> IsHealthyAsync(CancellationToken cancellationToken);
}
