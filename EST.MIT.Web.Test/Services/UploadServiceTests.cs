using System.Net;
using Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repositories;

namespace Services.Tests;

public class UploadServiceTests : TestContext
{
    private Mock<IUploadService> _uploadServiceMock;
    private Mock<IBlobService> _blobServiceMock;
    private Mock<IQueueService> _queueServiceMock;
    private Mock<ILogger<UploadService>> _logger;

    public RouteFields routeFields = new()
    {
        AccountType = "AC",
        Organisation = "ORG",
        PaymentType = "GB",
        SchemeType = "SchemeA",
        UserID = "henry.adetunji@defra.gov.uk"
    };

    public UploadServiceTests()
    {
        _uploadServiceMock = new Mock<IUploadService>();
        _blobServiceMock = new Mock<IBlobService>();
        _queueServiceMock = new Mock<IQueueService>();
        _logger = new Mock<ILogger<UploadService>>();
    }

    [Fact]
    public async Task UploadFileAsync_Returns_200()
    {
        Mock<IBrowserFile> fileMock = new Mock<IBrowserFile>();
        _blobServiceMock.Setup(x => x.AddFileToBlobAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IBrowserFile>())).Returns(Task.FromResult<bool>(true));
        _queueServiceMock.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<bool>(true));

        var uploadService = new UploadService(_logger.Object, _blobServiceMock.Object, _queueServiceMock.Object);

        var response = await uploadService.UploadFileAsync(fileMock.Object, routeFields.SchemeType, routeFields.Organisation, routeFields.PaymentType, routeFields.AccountType, routeFields.UserID);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

    }

    [Fact]
    public async Task UploadFileAsync_Returns_500()
    {
        Mock<IBrowserFile> fileMock = new Mock<IBrowserFile>();
        _blobServiceMock.Setup(x => x.AddFileToBlobAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IBrowserFile>(), It.IsAny<string>())).ThrowsAsync(new Exception("Blob Test Error"));

        var uploadService = new UploadService(_logger.Object, _blobServiceMock.Object, _queueServiceMock.Object);

        var response = await uploadService.UploadFileAsync(fileMock.Object, routeFields.SchemeType, routeFields.Organisation, routeFields.PaymentType, routeFields.AccountType, routeFields.UserID);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

    }

}