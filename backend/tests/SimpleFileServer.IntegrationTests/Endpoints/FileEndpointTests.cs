using Microsoft.EntityFrameworkCore;
using SimpleFileServer.Domain.Entities;
using SimpleFileServer.Infrastructure.EF;
using SimpleFileServer.IntegrationTests.Clients;
using SimpleFileServer.Web.Models;

namespace SimpleFileServer.IntegrationTests.Endpoints;

public sealed class FileEndpointTests : IClassFixture<CustomApplicationFactory>, IDisposable
{
    private readonly DomainFile testFile = new()
    {
        Id = Guid.Parse("a6803246-0220-4892-b09a-03e1f4b248b9"),
        Name = "file.txt",
        FileLocation = "test.txt",
        CreatedAt = DateTime.Parse("2026-09-09T00:00:00"),
        Size = 10,
    };
    private readonly string testFileContent = new('a', 10);

    private readonly CustomApplicationFactory appFactory;
    private readonly HttpClient httpClient;
    private readonly FileClient fileClient;
    private readonly IServiceScope serviceScope;
    private readonly AppDbContext dbContext;

    public FileEndpointTests(CustomApplicationFactory appFactory)
    {
        this.appFactory = appFactory;

        httpClient = appFactory.CreateClient();
        fileClient = new FileClient(httpClient);

        serviceScope = appFactory.Services.CreateScope();
        dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public void Dispose()
    {
        httpClient.Dispose();
        serviceScope.Dispose();
    }

    [Fact]
    public async Task UploadAsync_ShouldSaveFileInStoreAndDatabase()
    {
        await CleanRelatedDbStateAsync();

        const string fileName = "test.txt";
        using var fileContent = new MemoryStream([1, 2, 3]);

        IdModel response = await fileClient.UploadAsync(fileContent, fileName);

        DomainFile createdFileEntity = await dbContext.DomainFiles.AsNoTracking().SingleAsync(x => x.Id == response.Id);
        string physicalLocation = appFactory.GetFileStoragePath(createdFileEntity.FileLocation);

        Assert.Multiple(
            () => Assert.Equal(fileName, createdFileEntity?.Name),
            () => Assert.True(File.Exists(physicalLocation), $"Physical file \"{physicalLocation}\" does not exist.")
        );
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTestFiles()
    {
        await CleanRelatedDbStateAsync();
        await CreateTestFile();

        var expectedResult = new FileListModel([
            new FileInfoModel(testFile.Id, testFile.Name, testFile.Size, testFile.CreatedAt),
        ]);

        FileListModel result = await fileClient.GetAllAsync();

        Assert.Equivalent(expectedResult, result);
    }

    [Fact]
    public async Task GetContentAsync_ShouldReturnFileContent()
    {
        await CleanRelatedDbStateAsync();
        await CreateTestFile();

        using Stream fileStream = await fileClient.GetContentAsync(testFile.Id);

        using var streamReader = new StreamReader(fileStream);
        string fileContent = await streamReader.ReadToEndAsync();
        Assert.Equal(testFileContent, fileContent);
    }

    [Fact]
    public async Task RenameAsync_ShouldChangeFileName()
    {
        await CleanRelatedDbStateAsync();
        await CreateTestFile();

        const string newName = "newName.txt";

        await fileClient.RenameAsync(testFile.Id, newName);

        DomainFile fileEntry = dbContext.DomainFiles.AsNoTracking().First(x => x.Id == testFile.Id);
        Assert.Equal(newName, fileEntry.Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteFromDbAndStorage()
    {
        await CleanRelatedDbStateAsync();
        await CreateTestFile();

        await fileClient.DeleteAsync(testFile.Id);

        string filePath = appFactory.GetFileStoragePath(testFile.FileLocation);
        bool doesDbRowStillExist = await dbContext.DomainFiles.AnyAsync(x => x.Id == testFile.Id);
        Assert.Multiple(
            () => Assert.False(File.Exists(filePath), "Physical file still exists."),
            () => Assert.False(doesDbRowStillExist, "File row was not deleted.")
        );
    }

    private async Task CleanRelatedDbStateAsync()
    {
        await dbContext.DomainFiles.ExecuteDeleteAsync();
    }

    private async Task CreateTestFile()
    {
        dbContext.DomainFiles.Add(testFile);
        await dbContext.SaveChangesAsync();

        string filePath = appFactory.GetFileStoragePath(testFile.FileLocation);
        await File.WriteAllTextAsync(filePath, testFileContent);
    }
}
