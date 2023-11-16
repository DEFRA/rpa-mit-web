using System.Net;
using EST.MIT.Web.Repositories;
using Moq.Contrib.HttpClient;

namespace EST.MIT.Web.Test.Repositories;

public class ApprovalRepositoryTests : TestContext
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    public ApprovalRepositoryTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
    }

    [Fact]
    public async void GetApproversAsync_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new ApprovalRepository(factory);

        var response = await repo.GetApproversAsync("BPS", "123");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async void ValidateApprover_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new ApprovalRepository(factory);

        var response = await repo.ValidateApproverAsync("Loid.Forger@defra.gov.uk", "schemeType");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async void HandleHttpResponseError_Handed_FailCode()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.BadRequest, "Test BadRequest");

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new ApprovalRepository(factory);

        var response = await repo.GetApproversAsync("BPS", "123");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.ReadAsStringAsync().Result.Should().Be("Test BadRequest");
    }
}