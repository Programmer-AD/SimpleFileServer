using SimpleFileServer.Application.Abstractions.Infrastructure;
using SimpleFileServer.Application.Services;
using SimpleFileServer.Domain.Entities;
using SimpleFileServer.Domain.Exceptions;

namespace SimpleFileServer.UnitTests.Application.Services;

public class DomainFileServiceTests
{
    private static readonly Guid TestId = Guid.Parse("788f9a93-a0d3-4d89-a673-dc3efc92909a");
    private readonly DomainFile testFile = new()
    {
        Id = TestId,
        Name = "testName.txt",
        FileLocation = "testlocation",
        Size = 100,
        CreatedAt = DateTime.Parse("2026-09-09T00:00:00"),
    };

    private readonly Mock<IDomainFileRepository> fileRepositoryMock;
    private readonly Mock<IFileStore> fileStoreMock;
    private readonly Mock<TimeProvider> timeProviderMock;

    private readonly DomainFileService fileService;

    public DomainFileServiceTests()
    {
        fileRepositoryMock = new();
        fileStoreMock = new();
        timeProviderMock = new();

        fileService = new(
            fileRepositoryMock.Object,
            fileStoreMock.Object,
            timeProviderMock.Object);
    }

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_SavesEntity()
    {
        const string fileName = "newTest.txt";
        const string fileLocation = "fileLocation";
        DateTime creationTime = DateTime.Parse("2026-09-09T00:00:00Z").ToUniversalTime();
        using var contentStream = new MemoryStream([1, 2, 3]);

        timeProviderMock
            .Setup(x => x.GetUtcNow())
            .Returns(creationTime);

        fileStoreMock
            .Setup(x => x.StoreAsync(contentStream))
            .ReturnsAsync(fileLocation);

        DomainFile? createdFile = null;
        fileRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<DomainFile>()))
            .ReturnsAsync(TestId)
            .Callback((DomainFile file) => createdFile = file);
        var expectedFile = new DomainFile()
        {
            Name = fileName,
            FileLocation = fileLocation,
            CreatedAt = creationTime,
            Size = 3,
        };

        await fileService.CreateAsync(fileName, contentStream);

        Assert.Equivalent(expectedFile, createdFile);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnsAll()
    {
        List<DomainFile> expectedResult = [testFile];
        fileRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(expectedResult);

        List<DomainFile> result = await fileService.GetAllAsync();

        Assert.Equivalent(expectedResult, result);
    }

    #endregion

    #region GetAsync

    [Fact]
    public async Task GetAsync_WhenEntityNotFound_ThrowsDomainException()
    {
        SetFileEntityFound(false);

        DomainException exception = await Assert.ThrowsAsync<DomainException>(() => fileService.GetAsync(TestId));

        Assert.Equal(DomainExceptionTypes.GenericNotFound, exception.TypeName);
    }

    [Fact]
    public async Task GetAsync_WhenEntityFound_ReturnsEntity()
    {
        SetFileEntityFound(true);

        DomainFile result = await fileService.GetAsync(TestId);

        Assert.Equal(testFile, result);
    }

    #endregion

    #region GetContentAsync

    [Fact]
    public async Task GetContentAsync_WhenEntityNotFound_ThrowsDomainException()
    {
        SetFileEntityFound(false);

        DomainException exception = await Assert.ThrowsAsync<DomainException>(() => fileService.GetContentAsync(TestId));

        Assert.Equal(DomainExceptionTypes.GenericNotFound, exception.TypeName);
    }

    [Fact]
    public async Task GetContentAsync_WhenFileContentNotFound_ThrowsDomainException()
    {
        SetFileEntityFound(true);
        fileStoreMock
            .Setup(x => x.GetContent(testFile.FileLocation))
            .Returns((Stream?)null);

        DomainException exception = await Assert.ThrowsAsync<DomainException>(() => fileService.GetContentAsync(TestId));

        Assert.Equal(DomainExceptionTypes.GenericNotFound, exception.TypeName);
    }

    [Fact]
    public async Task GetContentAsync_WhenEverythingFound_ReturnsContent()
    {
        SetFileEntityFound(true);
        using var expectedStream = new MemoryStream([1, 2, 3]);
        fileStoreMock
            .Setup(x => x.GetContent(testFile.FileLocation))
            .Returns(expectedStream);

        Stream result = await fileService.GetContentAsync(TestId);

        Assert.Equal(expectedStream, result);

    }

    #endregion

    #region RenameAsync

    [Fact]
    public async Task RenameAsync_WhenEntityNotFound_ThrowsDomainException()
    {
        const string newName = "newName.txt";
        SetFileEntityFound(false);

        DomainException exception = await Assert.ThrowsAsync<DomainException>(() => fileService.RenameAsync(TestId, newName));

        Assert.Equal(DomainExceptionTypes.GenericNotFound, exception.TypeName);
    }

    [Fact]
    public async Task RenameAsync_WhenEntityFound_UpdatesName()
    {
        const string newName = "newName.txt";
        SetFileEntityFound(true);

        DomainFile? updatedFile = null;
        fileRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<DomainFile>()))
            .Callback((DomainFile file) => updatedFile = file);

        await fileService.RenameAsync(TestId, newName);

        Assert.Equal(newName, updatedFile?.Name);
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WhenEntityNotFound_DoesNothing()
    {
        SetFileEntityFound(false);

        await fileService.DeleteAsync(TestId);

        fileStoreMock.Verify(x => x.Delete(It.IsAny<string>()), Times.Never);
        fileRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityFound_DeletesEntity()
    {
        SetFileEntityFound(true);

        await fileService.DeleteAsync(TestId);

        fileStoreMock.Verify(x => x.Delete(testFile.FileLocation), Times.Once);
        fileRepositoryMock.Verify(x => x.DeleteAsync(TestId), Times.Once);

    }

    #endregion

    #region Utility methods

    private void SetFileEntityFound(bool isFound)
    {
        fileRepositoryMock
            .Setup(x => x.GetAsync(TestId))
            .ReturnsAsync(isFound ? testFile : null);
    }

    #endregion
}
