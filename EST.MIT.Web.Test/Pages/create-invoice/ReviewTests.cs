using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.create_invoice.Review;
using EST.MIT.Web.Shared;
using Repositories;
using EST.MIT.Web.Services;

namespace EST.MIT.Web.Tests.Pages;

public class ReviewPageTests : TestContext
{

    private readonly Invoice _invoice;
    private readonly Mock<IInvoiceAPI> _mockApiService;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    private readonly Mock<IMapper> _mockAutoMapper;
    public ReviewPageTests()
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
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns((Invoice?)null);
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<Review>();

        navigationManager?.Uri.Should().Be("http://localhost/create-invoice");
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
    public void Saves_Invoice_Navigates_To_Summary()
    {
        _mockApiService.Setup(x => x.SaveInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new ApiResponse<Invoice>(System.Net.HttpStatusCode.Created));
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<Review>();
        var saveAndContinueButton = component.FindAll("button[type='submit']");

        saveAndContinueButton[0].Click();

        navigationManager?.Uri.Should().Be($"http://localhost/invoice/summary/{_invoice.SchemeType}/{_invoice.Id}");
    }

    [Fact]
    public void Cancels_Invoice_Navigates_To_HomePage()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<Review>();
        var cancelButton = component.FindAll("a.govuk-link:contains('Cancel')");

        cancelButton[0].Click();

        navigationManager?.Uri.Should().Be("http://localhost/");
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
