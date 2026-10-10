using Microsoft.EntityFrameworkCore;
using SimpleFileServer.Domain.Entities;
using SimpleFileServer.Infrastructure.EF;
using SimpleFileServer.IntegrationTests.Clients;
using SimpleFileServer.Web.Models;

namespace SimpleFileServer.IntegrationTests.Endpoints;

public sealed class FileEndpointTests : IClassFixture<CustomApplicationFactory>, IDisposable
{
    private const string TestFileLocation = "test.txt";
    private readonly DomainFile testFile = new()
    {
        Id = Guid.Parse("a6803246-0220-4892-b09a-03e1f4b248b9"),
        Name = "file.txt",
        FileLocation = TestFileLocation,
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
    public async Task RegisterAsync_ShouldRegisterFileInDatabase()
    {
        const string expectedFileName = "testFile.txt";
        const long expectedFileSize = 100;
        await CleanRelatedDbStateAsync();

        IdModel response = await fileClient.RegisterAsync(expectedFileName, expectedFileSize);

        DomainFile createdFileEntity = await dbContext.DomainFiles.AsNoTracking().SingleAsync(x => x.Id == response.Id);
        Assert.Equivalent(new
        {
            Name = expectedFileName,
            Size = expectedFileSize,
            FileLocation = (string?)null,
        }, createdFileEntity);
    }

    [Fact]
    public async Task InitContentAsync_ShouldSaveFileContent()
    {
        await CleanRelatedDbStateAsync();
        await CreateTestFile(withContent: false);
        using var fileContent = new MemoryStream([1, 2, 3]);

        await fileClient.InitContentAsync(testFile.Id, fileContent);

        string physicalLocation = appFactory.GetFileStoragePath(TestFileLocation);
        Assert.True(File.Exists(physicalLocation), $"Physical file \"{physicalLocation}\" does not exist.");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTestFiles()
    {
        await CleanRelatedDbStateAsync();
        await CreateTestFile(withContent: true);

        var expectedResult = new FileListModel([
            new FileInfoModel(testFile.Id, testFile.Name, testFile.Size, testFile.CreatedAt, true),
        ]);

        FileListModel result = await fileClient.GetAllAsync();

        Assert.Equivalent(expectedResult, result);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnFile()
    {
        await CleanRelatedDbStateAsync();
        await CreateTestFile(withContent: false);

        FileInfoModel? result = await fileClient.GetAsync(testFile.Id);

        Assert.Equal(testFile.Name, result!.Name);
    }

    [Fact]
    public async Task GetContentAsync_WhenSpecifiedPhysicalFileIsMissing_ReturnsNotFound()
    {
        await CleanRelatedDbStateAsync();
        await CreateTestFile(withContent: true);
        string filePath = appFactory.GetFileStoragePath(TestFileLocation);
        File.Delete(filePath);

        HttpRequestException exception = await Assert.ThrowsAsync<HttpRequestException>(() => fileClient.GetContentAsync(testFile.Id));

        Assert.Equal(404, (int)exception.StatusCode!.Value);
    }

    [Fact]
    public async Task GetContentAsync_ShouldReturnFileContent()
    {
        await CleanRelatedDbStateAsync();
        await CreateTestFile(withContent: true);

        using Stream fileStream = await fileClient.GetContentAsync(testFile.Id);

        using var streamReader = new StreamReader(fileStream);
        string fileContent = await streamReader.ReadToEndAsync();
        Assert.Equal(testFileContent, fileContent);
    }

    [Fact]
    public async Task RenameAsync_ShouldChangeFileName()
    {
        await CleanRelatedDbStateAsync();
        await CreateTestFile(withContent: false);

        const string newName = "newName.txt";

        await fileClient.RenameAsync(testFile.Id, newName);

        DomainFile fileEntry = dbContext.DomainFiles.AsNoTracking().First(x => x.Id == testFile.Id);
        Assert.Equal(newName, fileEntry.Name);
    }

    [Fact]
    public async Task DeleteAsync_WhenFileEntityDoesNotExist_ShouldNotFail()
    {
        await CleanRelatedDbStateAsync();
        // Test File is not created

        await fileClient.DeleteAsync(testFile.Id);
    }

    [Fact]
    public async Task DeleteAsync_WhenPhysicalFileDoesNotExist_ShouldDeleteFromDb()
    {
        await CleanRelatedDbStateAsync();
        await CreateTestFile(withContent: false);

        await fileClient.DeleteAsync(testFile.Id);

        bool doesDbRowStillExist = await dbContext.DomainFiles.AnyAsync(x => x.Id == testFile.Id);
        Assert.False(doesDbRowStillExist, "File row was not deleted.");
    }

    [Fact]
    public async Task DeleteAsync_WhenEverythingExists_ShouldDeleteFromDbAndStorage()
    {
        await CleanRelatedDbStateAsync();
        await CreateTestFile(withContent: true);

        await fileClient.DeleteAsync(testFile.Id);

        string filePath = appFactory.GetFileStoragePath(TestFileLocation);
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

    private async Task CreateTestFile(bool withContent)
    {
        testFile.FileLocation = withContent ? TestFileLocation : null;
        dbContext.DomainFiles.Add(testFile);
        await dbContext.SaveChangesAsync();

        if (withContent)
        {
            string filePath = appFactory.GetFileStoragePath(TestFileLocation);
            await File.WriteAllTextAsync(filePath, testFileContent);
        }
    }
}
