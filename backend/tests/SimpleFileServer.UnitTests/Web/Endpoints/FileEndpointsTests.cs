using System.Net.Mime;
using Microsoft.AspNetCore.Http;
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
    public async Task UploadAsync_WhenFileNameContainsInvalidCharacters_ShouldSaveFileWithCleanedName()
    {
        const string originalFileName = "File\"3\"/.txt";
        const string expectedFileName = "File_3__.txt";
        using var formFileStream = new MemoryStream([1, 2, 3]);
        var formFile = new FormFile(formFileStream, 0, formFileStream.Length, "file", originalFileName);

        fileServiceMock
            .Setup(x => x.CreateAsync(expectedFileName, It.Is<Stream>(x => x.Length == formFileStream.Length)))
            .ReturnsAsync(TestId);

        Ok<IdModel> result = await FileEndpoints.UploadAsync(formFile, fileServiceMock.Object);

        Assert.Equal(TestId, result.Value?.Id);
    }

    [Fact]
    public async Task UploadAsync_WhenFileNameIsValid_ShouldSaveFileWithOriginalName()
    {
        const string fileName = "File3.txt";
        using var formFileStream = new MemoryStream([1, 2, 3]);
        var formFile = new FormFile(formFileStream, 0, formFileStream.Length, "file", fileName);

        fileServiceMock
            .Setup(x => x.CreateAsync(fileName, It.Is<Stream>(x => x.Length == formFileStream.Length)))
            .ReturnsAsync(TestId);

        Ok<IdModel> result = await FileEndpoints.UploadAsync(formFile, fileServiceMock.Object);

        Assert.Equal(TestId, result.Value?.Id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllFiles()
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
                    FileLocation = "testLocation2",
                    CreatedAt = DateTime.Parse("2026-09-02T00:00:00"),
                    Size = 102
                },
            ]);
        var expectedResult = new FileListModel([
            new (Guid.Parse("788f9a93-a0d3-4d89-a673-000000000001"), "testFile1.txt", 101, DateTime.Parse("2026-09-01T00:00:00")),
            new (Guid.Parse("788f9a93-a0d3-4d89-a673-000000000002"), "testFile2.txt", 102, DateTime.Parse("2026-09-02T00:00:00")),
        ]);

        Ok<FileListModel> result = await FileEndpoints.GetAllAsync(fileServiceMock.Object);
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
