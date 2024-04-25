using System.Net;
using System.Net.Http.Headers;
using EST.MIT.Web.Repositories;
using Moq.Contrib.HttpClient;

namespace EST.MIT.Web.Test.Repositories;

public class UploadRepositoryTests
{

    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

    public UploadRepositoryTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
    }

    [Fact]
    public async Task GetUploads_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new UploadRepository(factory);

        var response = await repo.GetUploads();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HandleHttpResponseError_Handed_FailCode()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.BadRequest, "Test BadRequest");

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new ReferenceDataRepository(factory);

        var response = await repo.GetSchemesListAsync("test", "test", "test", "test");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Test BadRequest");
    }

    [Fact]
    public async Task GetFileByFileNameAsync_ReturnsFileContent_WhenFileExists()    // Need to get files by ID, not filename
    {
        var requestId = Guid.NewGuid();
        var expectedContent = new ByteArrayContent(new byte[] { 1, 2, 3, 4 });
        expectedContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        _mockHttpMessageHandler.SetupRequest(HttpMethod.Get, $"https://localhost/UploadedFile/{requestId}")
                            .ReturnsResponse(HttpStatusCode.OK, expectedContent);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new UploadRepository(factory);

        var response = await repo.GetFileByImportRequestIdAsync(requestId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsByteArrayAsync();
        content.Should().Equal(new byte[] { 1, 2, 3, 4 });
        response.Content.Headers.ContentType.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [Fact]
    public async Task GetFileByFileNameAsync_ReturnsNull_WhenFileNotFound()
    {
        var nonExistantRequestId = Guid.NewGuid();
        _mockHttpMessageHandler.SetupRequest(HttpMethod.Get, $"https://localhost/UploadedFile/{nonExistantRequestId}")
                            .ReturnsResponse(HttpStatusCode.NotFound);

        var factory = _mockHttpMessageHandler.CreateClientFactory();
        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });
        var repo = new UploadRepository(factory);

        var response = await repo.GetFileByImportRequestIdAsync(nonExistantRequestId);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().BeEmpty();
    }
}