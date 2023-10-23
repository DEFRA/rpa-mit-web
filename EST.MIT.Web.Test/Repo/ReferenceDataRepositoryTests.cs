using System.Net;
using Moq.Contrib.HttpClient;

namespace Repositories.Tests;

public class ReferenceDataRepositoryTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

    public ReferenceDataRepositoryTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
    }

    [Fact]
    public void GetOrganisationsListAsync_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new ReferenceDataRepository(factory);

        var response = repo.GetOrganisationsListAsync();

        response.Result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void GetSchemeTypesListAsync_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new ReferenceDataRepository(factory);

        var response = repo.GetSchemeTypesListAsync("test", "test");

        response.Result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void GetFundListAsync_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new ReferenceDataRepository(factory);

        var response = repo.GetFundsListAsync("test", "test", "test", "test");

        response.Result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void HandleHttpResponseError_Handed_FailCode()
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

        var response = repo.GetSchemeTypesListAsync("test", "test");

        response.Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Result.Content.ReadAsStringAsync().Result.Should().Be("Test BadRequest");

    }
}