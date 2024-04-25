using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.invoice.DeletePaymentRequest;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

public class DeletePaymentRequestTests : TestContext
{
    private readonly Invoice _invoice;
    private readonly Mock<IInvoiceAPI> _mockApiService;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    public string backUrl = "user-invoices";

    public DeletePaymentRequestTests()
    {
        _invoice = new Invoice()
        {
            Id = Guid.NewGuid(),
            PaymentRequests = new List<PaymentRequest>()
        };

        _mockApiService = new Mock<IInvoiceAPI>();
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();

        Services.AddSingleton<IInvoiceAPI>(_mockApiService.Object);
        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);

    }

    [Fact]
    public void ConfirmDelete_Success_Navigates_To_Summary_No_Invoice_Returned()
    {
        _mockInvoiceStateContainer.Setup(x => x.Value).Returns(_invoice);
        _mockApiService.Setup(x => x.DeletePaymentRequestAsync(It.IsAny<Invoice>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));

        var component = RenderComponent<DeletePaymentRequest>(parameter =>
        {
            parameter.Add(p => p.PaymentRequestId, "1");
        });

        var confirmButton = component.FindAll("button.govuk-button")[0];

        confirmButton.Click();

        var navigationManager = Services.GetService<NavigationManager>();
        component.WaitForAssertion(() => navigationManager.Uri.Should().Be($"http://localhost/invoice/summary/{_invoice.SchemeType}/{_invoice.Id}"));
    }

    [Fact]
    public void ConfirmDelete_Success_Navigates_To_Summary()
    {
        _mockInvoiceStateContainer.Setup(x => x.Value).Returns(_invoice);
        _mockApiService.Setup(x => x.DeletePaymentRequestAsync(It.IsAny<Invoice>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK) { Data = _invoice });

        var component = RenderComponent<DeletePaymentRequest>(parameter =>
        {
            parameter.Add(p => p.PaymentRequestId, "1");
        });

        var confirmButton = component.FindAll("button.govuk-button")[0];

        confirmButton.Click();

        var navigationManager = Services.GetService<NavigationManager>();
        component.WaitForAssertion(() => navigationManager.Uri.Should().Be($"http://localhost/invoice/summary/{_invoice.SchemeType}/{_invoice.Id}"));
    }

    [Fact]
    public void ConfirmDelete_Navigates_To_Summary_On_Cancel()
    {
        _mockInvoiceStateContainer.Setup(x => x.Value).Returns(_invoice);

        var component = RenderComponent<DeletePaymentRequest>(parameter =>
        {
            parameter.Add(p => p.PaymentRequestId, "1");
        });

        var link = component.FindAll("a.govuk-link")[0];

        link.Click();

        var navigationManager = Services.GetService<NavigationManager>();
        component.WaitForAssertion(() => navigationManager.Uri.Should().Be($"http://localhost/invoice/summary/{_invoice.SchemeType}/{_invoice.Id}/{backUrl}"));
    }
}