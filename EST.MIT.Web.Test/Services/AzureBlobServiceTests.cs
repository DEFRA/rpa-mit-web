using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EST.MIT.Web.Services;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Test.Services;

public class AzureBlobServiceTests : TestContext
{

    private readonly IConfiguration _configuration;
    private readonly IBrowserFile _file;
    private readonly IAzureBlobService _blobService;
    private Mock<BlobContainerClient> mockBlobContainerClient;
    private Mock<Response<BlobContentInfo>> responseMock;
    private ILogger<IAzureBlobService> _logger;

    public AzureBlobServiceTests()
    {
        var blobClientMock = new Mock<BlobServiceClient>();
        var configMock = new Mock<IConfiguration>();
        var configSectionMock = new Mock<IConfigurationSection>();

        configSectionMock.Setup(x => x.Value).Returns("connection_string");
        configMock.Setup(x => x.GetSection(It.Is<string>(s => s == "ConnectionStrings:PrimaryConnection"))).Returns(configSectionMock.Object);
        _configuration = configMock.Object;

        _logger = Mock.Of<ILogger<IAzureBlobService>>();
        _file = Mock.Of<IBrowserFile>();

        var mockBlobServiceClient = new Mock<BlobServiceClient>();
        mockBlobContainerClient = new Mock<BlobContainerClient>();
        var mockAzureBlobService = new Mock<IAzureBlobService>();
        responseMock = new Mock<Response<BlobContentInfo>>();

        mockAzureBlobService.Setup(x => x.blobServiceClient).Returns(mockBlobServiceClient.Object);
        mockBlobContainerClient.Setup(x => x.CreateIfNotExistsAsync(PublicAccessType.BlobContainer, null, null, default)).ReturnsAsync(new Mock<Response<BlobContainerInfo>>().Object);
        mockBlobContainerClient.Setup(x => x.UploadBlobAsync(It.IsAny<string>(), It.IsAny<Stream>(), default)).ReturnsAsync(responseMock.Object);
        mockBlobServiceClient.Setup(x => x.GetBlobContainerClient(It.IsAny<string>())).Returns(mockBlobContainerClient.Object);

        blobClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>())).Returns(mockBlobContainerClient.Object);

        _blobService = new AzureBlobService(blobClientMock.Object, _logger, AzureBlobService.default_BlobContainerName);
    }

    [Fact]
    public async void AddFileToBlobAsync_Upload_Successful()
    {
        string blobName = "test-blob";

        mockBlobContainerClient.Setup(x => x.ExistsAsync(default)).ReturnsAsync(Response.FromValue(true, new Mock<Response>().Object));

        var result = await _blobService.AddFileToBlobAsync(blobName, _file);

        result.Should().BeTrue();
    }

    [Fact]
    public async void AddFileToBlobAsync_Container_Not_Exists()
    {
        string blobName = "test-blob";

        mockBlobContainerClient.Setup(x => x.ExistsAsync(default)).ReturnsAsync(Response.FromValue(false, new Mock<Response>().Object));

        var result = await _blobService.AddFileToBlobAsync(blobName, _file);

        result.Should().BeFalse();
    }

    [Fact]
    public async void AddFileToBlobAsync_Upload_With_Directory_Successful()
    {
        string blobFileName = "folder/test-blob";

        mockBlobContainerClient.Setup(x => x.ExistsAsync(default)).ReturnsAsync(Response.FromValue(true, new Mock<Response>().Object));

        var result = await _blobService.AddFileToBlobAsync(blobFileName, _file);

        result.Should().BeTrue();
    }
}