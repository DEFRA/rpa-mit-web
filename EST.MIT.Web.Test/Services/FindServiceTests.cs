using Entities;
using Repositories;

namespace Services.Tests;

public class FindServiceTests : TestContext
{
    [Fact]
    public void FetchInvoiceAsync()
    {
        var mockAPIService = new Mock<IInvoiceAPI>();
        mockAPIService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Invoice());

        var service = new FindService(mockAPIService.Object, Mock.Of<IReferenceDataRepository>());

        var response = service.FetchInvoiceAsync("", "");
        response.Should().NotBeNull();
    }

    [Fact]
    public void FetchInvoiceReturnsNull()
    {
        var mockAPIService = new Mock<IInvoiceAPI>();
        mockAPIService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<Invoice>(null));

        var service = new FindService(mockAPIService.Object, Mock.Of<IReferenceDataRepository>());

        var response = service.FetchInvoiceAsync("", "").Result;
        response.Should().BeNull();
    }

}
