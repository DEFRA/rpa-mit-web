using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.invoice.ViewPaymentRequestSummary;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

public class ViewPaymentRequestSummaryTests : TestContext
{
    private readonly Mock<IInvoiceAPI> _mockApiService;
    private readonly Mock<IApprovalService> _mockApprovalService;
    private Invoice _invoice;
    public ViewPaymentRequestSummaryTests()
    {

        _invoice = new Invoice()
        {
            PaymentRequests = new List<PaymentRequest>() { new PaymentRequest() }
        };

        var mockInvoiceRepository = new Mock<IInvoiceRepository>();
        _mockApiService = new Mock<IInvoiceAPI>();
        _mockApprovalService = new Mock<IApprovalService>();

        Services.AddSingleton<IInvoiceRepository>(mockInvoiceRepository.Object);
        Services.AddSingleton<IInvoiceAPI>(_mockApiService.Object);
        Services.AddSingleton<IApprovalService>(_mockApprovalService.Object);
    }

    [Fact]
    public void InvoiceId_Is_Displayed()
    {
        _mockApprovalService.Setup(x => x.GetInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_invoice);

        var component = RenderComponent<ViewPaymentRequestSummary>();

        var Title = component.FindAll(".govuk-heading-l")[0];

        Title.InnerHtml.Should().NotBeNull();

    }

    [Fact]
    public void Invoice_Is_Not_Found()
    {
        _mockApprovalService.Setup(x => x.GetInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult<Invoice>(null!));

        var component = RenderComponent<ViewPaymentRequestSummary>();

        var renderedErrors = component.FindAll("ul.govuk-error-summary__list > li");

        renderedErrors.Count.Should().Be(1);
    }

    [Fact]
    public void OnInitializedAsync_Fails_Get()
    {
        _mockApprovalService.Setup(x => x.GetInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new Exception("Test Exception"));

        var component = RenderComponent<ViewPaymentRequestSummary>();

        var renderedErrors = component.FindAll("ul.govuk-error-summary__list > li");

        renderedErrors.Count.Should().Be(1);
        renderedErrors[0].FirstElementChild.InnerHtml.Should().Be("Test Exception");
    }

}