using EST.MIT.Web.Entities;
using EST.MIT.Web.Services;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Test.Services;

public class FindServiceTests : TestContext
{
    [Fact]
    public void FetchInvoiceAsync()
    {
        var mockAPIService = new Mock<IInvoiceAPI>();
        mockAPIService.Setup(x => x.FindInvoiceAsync(It.IsAny<SearchCriteria>())).ReturnsAsync(new Invoice());

        var service = new FindService(mockAPIService.Object);

        var response = service.FindInvoiceAsync(new SearchCriteria());
        response.Should().NotBeNull();
    }

    [Fact]
    public async Task FetchInvoiceReturnsNull()
    {
        var mockAPIService = new Mock<IInvoiceAPI>();
        mockAPIService.Setup(x => x.FindInvoiceAsync(It.IsAny<SearchCriteria>())).Returns(Task.FromResult<Invoice>(null));

        var service = new FindService(mockAPIService.Object);

        var response = await service.FindInvoiceAsync(new SearchCriteria());
        response.Should().BeNull();
    }
}