using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.invoice.ReadonlyInvoiceSummary;
using EST.MIT.Web.Repositories;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

public class ReadonlyInvoiceSummaryTests : TestContext
{
    private readonly Mock<IInvoiceAPI> _mockApiService;
    private readonly Mock<IApprovalService> _mockApprovalService;
    private Invoice _invoice;
    public ReadonlyInvoiceSummaryTests()
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

        var component = RenderComponent<ReadonlyInvoiceSummary>();

        var Title = component.FindAll(".govuk-heading-l")[0];

        Title.InnerHtml.Should().NotBeNull();

    }

    [Fact]
    public void ApproveInvoice_Runs_ApproveInvoiceAsync()
    {
        _mockApprovalService.Setup(x => x.GetApprovalAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_invoice);

        var component = RenderComponent<ReadonlyInvoiceSummary>(parameters =>
        {
            parameters.Add(x => x.Approval, true);
        });

        component.FindAll("button#ApproveInvoice")[0].Click();

        _mockApprovalService.Verify(x => x.ApproveInvoiceAsync(It.IsAny<Invoice>()), Times.Once);

    }

    [Fact]
    public void RejectInvoice_Runs_RejectInvoiceAsync()
    {
        _mockApprovalService.Setup(x => x.GetApprovalAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_invoice);

        var component = RenderComponent<ReadonlyInvoiceSummary>(parameters =>
        {
            parameters.Add(x => x.Approval, true);
        });

        component.FindAll("button#RejectInvoice")[0].Click();

        _mockApprovalService.Verify(x => x.RejectInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<string>()), Times.Once);

    }

    [Fact]
    public void Invoice_Is_Not_Found()
    {
        _mockApprovalService.Setup(x => x.GetInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult<Invoice>(null));

        var component = RenderComponent<ReadonlyInvoiceSummary>();

        var renderedErrors = component.FindAll("ul.govuk-error-summary__list > li");

        renderedErrors.Count.Should().Be(1);
    }

    [Fact]
    public void OnInitializedAsync_Fails_Get()
    {
        _mockApprovalService.Setup(x => x.GetInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new Exception("Test Exception"));

        var component = RenderComponent<ReadonlyInvoiceSummary>();

        var renderedErrors = component.FindAll("ul.govuk-error-summary__list > li");

        renderedErrors.Count.Should().Be(1);
        renderedErrors[0].FirstElementChild.InnerHtml.Should().Be("Test Exception");
    }

}