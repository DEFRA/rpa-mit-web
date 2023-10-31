using System.Net;
using AutoMapper;
using EST.MIT.Web.Entities;
using Moq.Contrib.HttpClient;

namespace Repositories.Tests;

public class InvoiceRepositoryTests : TestContext
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<IMapper> _mockAutoMapper;
    public InvoiceRepositoryTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockAutoMapper = new Mock<IMapper>();

        // If you wanted to use the real mapper, you could do this:
        // var mapperConfig = new MapperConfiguration(mc =>
        // {
        //     mc.AddProfile(new InvoiceAPIMapper());
        // });
        // IMapper mapper = mapperConfig.CreateMapper();
        //
        // And then pass the mapper into the repository
        // var repo = new InvoiceRepository(factory, mapper);
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

        var repo = new InvoiceRepository(factory, this._mockAutoMapper.Object);

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

        var repo = new InvoiceRepository(factory, this._mockAutoMapper.Object);

        var response = await repo.PostInvoiceAsync(new Invoice());

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

        var repo = new InvoiceRepository(factory, this._mockAutoMapper.Object);

        var response = await repo.PutInvoiceAsync(new Invoice());

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

        var repo = new InvoiceRepository(factory, this._mockAutoMapper.Object);

        var response = await repo.DeleteHeaderAsync(new PaymentRequest());

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

        var repo = new InvoiceRepository(factory, this._mockAutoMapper.Object);

        var response = await repo.GetApprovalAsync("BLK-1234567", "test");

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

        var repo = new InvoiceRepository(factory, this._mockAutoMapper.Object);

        var response = await repo.GetApprovalsAsync();

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

        var repo = new InvoiceRepository(factory, this._mockAutoMapper.Object);

        var response = await repo.PostInvoiceAsync(new Invoice());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.ReadAsStringAsync().Result.Should().Be("Test BadRequest");
    }
}