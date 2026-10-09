using System.Net.Mime;
using Microsoft.AspNetCore.Http.HttpResults;
using SimpleFileServer.Application.Abstractions.Services;
using SimpleFileServer.Domain.Entities;
using SimpleFileServer.Web.Endpoints;
using SimpleFileServer.Web.Models;

namespace SimpleFileServer.UnitTests.Web.Endpoints;

public class FileEndpointsTests
{
    private static readonly Guid TestId = Guid.Parse("788f9a93-a0d3-4d89-a673-dc3efc92909a");

    private readonly Mock<IDomainFileService> fileServiceMock;

    public FileEndpointsTests()
    {
        fileServiceMock = new();
    }

    [Fact]
    public async Task RegisterAsync_ShouldRegisterFile()
    {
        const string name = "fileName.txt";
        const long size = 10201;

        await FileEndpoints.RegisterAsync(new(name, size), fileServiceMock.Object);

        fileServiceMock.Verify(x => x.RegisterAsync(name, size), Times.Once);
    }

    [Fact]
    public async Task InitContentAsync_ShouldInitContent()
    {
        using var fileStream = new MemoryStream([1, 2, 3]);

        await FileEndpoints.InitContentAsync(TestId, fileStream, fileServiceMock.Object);

        fileServiceMock.Verify(x => x.InitContentAsync(TestId, fileStream), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllFilesInfo()
    {
        fileServiceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync([
                new()
                {
                    Id = Guid.Parse("788f9a93-a0d3-4d89-a673-000000000001"),
                    Name = "testFile1.txt",
                    FileLocation = "testLocation1",
                    CreatedAt = DateTime.Parse("2026-09-01T00:00:00"),
                    Size = 101
                },
                new()
                {
                    Id = Guid.Parse("788f9a93-a0d3-4d89-a673-000000000002"),
                    Name = "testFile2.txt",
                    FileLocation = null,
                    CreatedAt = DateTime.Parse("2026-09-02T00:00:00"),
                    Size = 102
                },
            ]);
        var expectedResult = new FileListModel([
            new (Guid.Parse("788f9a93-a0d3-4d89-a673-000000000001"), "testFile1.txt", 101, DateTime.Parse("2026-09-01T00:00:00"), true),
            new (Guid.Parse("788f9a93-a0d3-4d89-a673-000000000002"), "testFile2.txt", 102, DateTime.Parse("2026-09-02T00:00:00"), false),
        ]);

        Ok<FileListModel> result = await FileEndpoints.GetAllAsync(fileServiceMock.Object);

        Assert.Equivalent(expectedResult, result.Value);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnFileInfo()
    {
        var createdAt = DateTime.Parse("2026-09-09T00:00:00");
        var file = new DomainFile()
        {
            Id = TestId,
            Name = "testName.txt",
            FileLocation = "testlocation",
            Size = 100,
            CreatedAt = createdAt,
        };

        fileServiceMock
            .Setup(x => x.GetAsync(TestId))
            .ReturnsAsync(file);

        var expectedResult = new FileInfoModel(TestId, "testName.txt", 100, createdAt, true);

        Ok<FileInfoModel> result = await FileEndpoints.GetAsync(TestId, fileServiceMock.Object);

        Assert.Equivalent(expectedResult, result.Value);
    }

    [Fact]
    public async Task GetContentAsync_ShouldReturnFileContent()
    {
        var file = new DomainFile()
        {
            Id = TestId,
            Name = "testName.txt",
            FileLocation = "testlocation",
            Size = 100,
            CreatedAt = DateTime.Parse("2026-09-09T00:00:00"),
        };
        using Stream expectedStream = new MemoryStream([1, 2, 3]);

        fileServiceMock
            .Setup(x => x.GetAsync(TestId))
            .ReturnsAsync(file);
        fileServiceMock
            .Setup(x => x.GetContentAsync(TestId))
            .ReturnsAsync(expectedStream);

        FileStreamHttpResult result = await FileEndpoints.GetContentAsync(TestId, fileServiceMock.Object);

        Assert.Multiple(
            () => Assert.Equal(expectedStream, result.FileStream),
            () => Assert.Equal(MediaTypeNames.Application.Octet, result.ContentType),
            () => Assert.Equal(file.Name, result.FileDownloadName)
        );
    }

    [Fact]
    public async Task RenameAsync_ShouldRenameFile()
    {
        const string newName = "newName.txt";

        await FileEndpoints.RenameAsync(TestId, new(newName), fileServiceMock.Object);

        fileServiceMock.Verify(x => x.RenameAsync(TestId, newName), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteFile()
    {
        await FileEndpoints.DeleteAsync(TestId, fileServiceMock.Object);

        fileServiceMock.Verify(x => x.DeleteAsync(TestId), Times.Once);
    }
}
