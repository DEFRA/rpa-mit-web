using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Entities;
using EST.MIT.Web.Pages.create_bulk.Review;
using EST.MIT.Web.Shared;
using Repositories;
using Services;

namespace Pages.Tests;

public class ReviewPageBulkTests : TestContext
{

    private readonly Invoice _invoice;
    private readonly Mock<IInvoiceAPI> _mockApiService;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    private readonly Mock<IMapper> _mockAutoMapper;
    public ReviewPageBulkTests()
    {
        _mockApiService = new Mock<IInvoiceAPI>();
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();
        _mockAutoMapper = new Mock<IMapper>();

        Services.AddSingleton<IPageServices, PageServices>();
        Services.AddSingleton<IInvoiceAPI>(_mockApiService.Object);
        Services.AddSingleton<IInvoiceRepository>(new InvoiceRepository(new Mock<IHttpClientFactory>().Object, _mockAutoMapper.Object));
        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);

        _invoice = new()
        {
            AccountType = "AR",
            PaymentType = "DOM",
            Organisation = "RPA",
            SchemeType = "BPS"
        };
    }

    [Fact]
    public void AfterRender_Redirects_When_Null_Invoice()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns((Invoice)null);
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<Review>();

        navigationManager?.Uri.Should().Be("http://localhost/create-bulk");
    }

    [Fact]
    public void Invoice_Values_Displayed_Correctly()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);

        var component = RenderComponent<Review>();
        var reviewInvoiceValues = component.FindAll("dd.govuk-summary-list__value");

        reviewInvoiceValues.Should().NotBeEmpty();
        reviewInvoiceValues.Should().HaveCount(4);
        reviewInvoiceValues[0].TextContent.Should().Be("AR");
        reviewInvoiceValues[1].TextContent.Should().Be("RPA");
        reviewInvoiceValues[2].TextContent.Should().Be("BPS");
        reviewInvoiceValues[3].TextContent.Should().Be("DOM");
    }
}

 