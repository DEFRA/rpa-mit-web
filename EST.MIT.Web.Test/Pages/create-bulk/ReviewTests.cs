using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.create_bulk.Review;
using EST.MIT.Web.Repositories;
using EST.MIT.Web.Services;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

public class ReviewPageBulkTests : TestContext
{

    private readonly Invoice _invoice;
    private readonly Mock<IInvoiceAPI> _mockApiService;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    public ReviewPageBulkTests()
    {
        _mockApiService = new Mock<IInvoiceAPI>();
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();

        Services.AddSingleton<IPageServices, PageServices>();
        Services.AddSingleton<IInvoiceAPI>(_mockApiService.Object);
        Services.AddSingleton<IInvoiceRepository>(new InvoiceRepository(new Mock<IHttpClientFactory>().Object));
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
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns((Invoice?)null);
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

    [Fact]
    public void Continue_Navigates_To_Bulk_Upload_Page()
    {
        _mockApiService.Setup(x => x.SaveInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new ApiResponse<Invoice>(System.Net.HttpStatusCode.Created));
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<Review>();
        var saveAndContinueButton = component.FindAll("button[type='submit']");
        saveAndContinueButton[0].Click();

        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be($"http://localhost/bulk/"));
    }

    [Fact]
    public void Cancels_Invoice_Navigates_To_HomePage()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<Review>();
        var cancelButton = component.FindAll("a.govuk-link:contains('Cancel')");

        cancelButton[0].Click();

        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be("http://localhost/"));
    }

    [Fact]
    public void Validation_Returns_Errors()
    {
        _invoice.AccountType = default!;
        _mockApiService.Setup(x => x.SaveInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new ApiResponse<Invoice>(System.Net.HttpStatusCode.Created));

        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<Review>();
        component.FindAll("button[type='submit']")[0].Click();

        component.WaitForElements("p.govuk-error-message");

        var errorMessages = component.FindAll("p.govuk-error-message");

        errorMessages.Should().NotBeEmpty();
        errorMessages.Should().HaveCount(1);
        errorMessages[0].TextContent.Should().Be("Account Type is required");
    }
}