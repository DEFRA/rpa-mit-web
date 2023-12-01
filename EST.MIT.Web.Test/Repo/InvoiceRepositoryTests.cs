using System.Net;
using EST.MIT.Web.DTOs;
using EST.MIT.Web.Repositories;
using Moq.Contrib.HttpClient;

namespace EST.MIT.Web.Test.Repositories;

public class InvoiceRepositoryTests : TestContext
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    public InvoiceRepositoryTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
    }

    [Fact]
    public async void GetInvoiceById_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new InvoiceRepository(factory);

        var response = await repo.GetInvoiceByIdAsync(Guid.NewGuid().ToString());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async void GetInvoice_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new InvoiceRepository(factory);

        var response = await repo.GetInvoiceAsync("BLK-1234567", "test");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async void PostInvoiceAsync_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new InvoiceRepository(factory);

        var response = await repo.PostInvoiceAsync(new PaymentRequestsBatchDTO());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async void PutInvoiceAsync_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new InvoiceRepository(factory);

        var response = await repo.PutInvoiceAsync(new PaymentRequestsBatchDTO());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async void DeleteHeaderAsync_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new InvoiceRepository(factory);

        var response = await repo.DeleteHeaderAsync(new PaymentRequestDTO());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async void GetApprovalAsync_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new InvoiceRepository(factory);

        var response = await repo.GetApprovalAsync("BLK-1234567");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async void GetApprovalsAsync_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new InvoiceRepository(factory);

        var response = await repo.GetAllApprovalsAsync();

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

        var repo = new InvoiceRepository(factory);

        var response = await repo.PostInvoiceAsync(new PaymentRequestsBatchDTO());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Be("Test BadRequest");
    }

    [Fact]
    public async Task GetInvoicesAsync_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new InvoiceRepository(factory);

        var response = await repo.GetInvoicesAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNull();
    }

    [Fact]
    public async Task GetInvoiceAsync_Returns_Correct_Content()
    {
        var expectedContent = "{\"id\":\"BLK-1234567\",\"status\":\"Approved\"}";
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK, expectedContent);

        var factory = new Mock<IHttpClientFactory>();

        factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var invoiceRepository = new InvoiceRepository(factory.Object);

        var response = await invoiceRepository.GetInvoiceAsync("BLK-1234567", "test");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().Be(expectedContent);
    }
}