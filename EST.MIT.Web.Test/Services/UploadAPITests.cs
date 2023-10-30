using System.Net;
using EST.MIT.Web.Entities;
using Microsoft.Extensions.Logging;
using Repositories;
using EST.MIT.Web.Services;

namespace EST.MIT.Web.Test.Services;

public class UploadAPITests
{
    private readonly Mock<IUploadRepository> _mockUploadRepository;
    private readonly Mock<ILogger<UploadAPI>> _mockLogger;
    private readonly UploadAPI _uploadAPI;

    public UploadAPITests()
    {
        _mockUploadRepository = new Mock<IUploadRepository>();
        _mockLogger = new Mock<ILogger<UploadAPI>>();
        _uploadAPI = new UploadAPI(_mockUploadRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetUploadsAsync_ReturnsData_WhenApiResponseIsSuccess()
    {
        var importRequests = new List<ImportRequest> { new ImportRequest { FileName = "test.xlsx" } };
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(importRequests))
        };
        _mockUploadRepository.Setup(repo => repo.GetUploads()).ReturnsAsync(httpResponseMessage);
        var result = await _uploadAPI.GetUploadsAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("test.xlsx", result.First().FileName);
    }

    [Fact]
    public async Task GetUploadsAsync_LogsWarningAndReturnsNull_WhenApiResponseHasNoContent()
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(string.Empty) };
        _mockUploadRepository.Setup(repo => repo.GetUploads()).ReturnsAsync(httpResponseMessage);
        var result = await _uploadAPI.GetUploadsAsync();

        Assert.Null(result);
        _mockLogger.Verify(logger => logger.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => string.Equals("API returned no data", o.ToString())),
            null,
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
        Times.Once);
    }

    [Fact]
    public async Task GetUploadsAsync_ReturnsNull_WhenApiResponseIsNotFound()
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
        _mockUploadRepository.Setup(repo => repo.GetUploads()).ReturnsAsync(httpResponseMessage);
        var result = await _uploadAPI.GetUploadsAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUploadsAsync_LogsErrorAndReturnsNull_WhenApiResponseIsUnknown()
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        _mockUploadRepository.Setup(repo => repo.GetUploads()).ReturnsAsync(httpResponseMessage);
        var result = await _uploadAPI.GetUploadsAsync();

        Assert.Null(result);
        _mockLogger.Verify(logger => logger.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }

}