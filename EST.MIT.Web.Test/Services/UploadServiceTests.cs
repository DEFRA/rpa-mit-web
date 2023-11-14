using System.Net;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;

namespace EST.MIT.Web.Test.Services;

public class UploadServiceTests : TestContext
{
    private Mock<IUploadService> _uploadServiceMock;
    private Mock<IAzureBlobService> _blobServiceMock;
    private Mock<IEventQueueService> _EventQueueServiceMock;
    private Mock<IImporterQueueService> _ImporterQueueServiceMock;
    private Mock<ILogger<UploadService>> _logger;

    Invoice routeFields = new()
    {
        AccountType = "AC",
        Organisation = "ORG",
        PaymentType = "GB",
        SchemeType = "SchemeA"
    };

    public UploadServiceTests()
    {
        _uploadServiceMock = new Mock<IUploadService>();
        _blobServiceMock = new Mock<IAzureBlobService>();
        _EventQueueServiceMock = new Mock<IEventQueueService>();
        _ImporterQueueServiceMock = new Mock<IImporterQueueService>();
        _logger = new Mock<ILogger<UploadService>>();
    }

    [Fact]
    public async Task UploadFileAsync_Returns_200()
    {


        Mock<IBrowserFile> fileMock = new Mock<IBrowserFile>();
        _blobServiceMock.Setup(x => x.AddFileToBlobAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IBrowserFile>())).Returns(Task.FromResult<bool>(true));
        _EventQueueServiceMock.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<bool>(true));
        _ImporterQueueServiceMock.Setup(x => x.AddMessageToQueueAsync(It.IsAny<ImportRequest>())).Returns(Task.FromResult<bool>(true));

        var uploadService = new UploadService(_logger.Object, _blobServiceMock.Object, _EventQueueServiceMock.Object, _ImporterQueueServiceMock.Object);

        var response = await uploadService.UploadFileAsync(fileMock.Object, routeFields.SchemeType, routeFields.Organisation, routeFields.PaymentType, routeFields.AccountType, "user");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

    }

    [Fact]
    public async Task UploadFileAsync_Returns_500()
    {
        Mock<IBrowserFile> fileMock = new Mock<IBrowserFile>();
        _blobServiceMock.Setup(x => x.AddFileToBlobAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IBrowserFile>(), It.IsAny<string>())).ThrowsAsync(new Exception("Blob Test Error"));
        _ImporterQueueServiceMock.Setup(x => x.AddMessageToQueueAsync(It.IsAny<ImportRequest>())).Returns(Task.FromResult<bool>(true));

        var uploadService = new UploadService(_logger.Object, _blobServiceMock.Object, _EventQueueServiceMock.Object, _ImporterQueueServiceMock.Object);

        var response = await uploadService.UploadFileAsync(fileMock.Object, routeFields.SchemeType, routeFields.Organisation, routeFields.PaymentType, routeFields.AccountType, "user");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}