using EST.MIT.Web.Entities;
using EST.MIT.Web.Services;
using EST.MIT.Web.Repositories;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Test.Services;

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
    public async Task FetchInvoiceReturnsNull()
    {
        var mockAPIService = new Mock<IInvoiceAPI>();
        mockAPIService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<Invoice>(null));

        var service = new FindService(mockAPIService.Object, Mock.Of<IReferenceDataRepository>());

        var response = await service.FetchInvoiceAsync("", "");
        response.Should().BeNull();
    }
}