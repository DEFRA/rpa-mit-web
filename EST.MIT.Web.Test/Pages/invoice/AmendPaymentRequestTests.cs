using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Entities;
using EST.MIT.Web.Pages.invoice.AmendPaymentRequest;
using EST.MIT.Web.Shared;

namespace Pages.Tests;

public class AmendPaymentRequestTests : TestContext
{
    private readonly Invoice _invoice;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;

    public AmendPaymentRequestTests()
    {
        _invoice = new Invoice();
        _invoice.PaymentRequests.Add(new PaymentRequest()
        {
            PaymentRequestId = "1",
            CustomerId = "1234567890",
            Value = 0,
            Currency = "GBP",
            InvoiceLines = new List<InvoiceLine>()
        });

        _invoice.PaymentRequests[0].InvoiceLines.Add(new InvoiceLine()
        {
            Value = 100.00,
            DeliveryBody = "RP00",
            SchemeCode = "BPS",
            Description = "G00 - Gross Value"
        });

        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();
        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
    }

    [Fact]
    public void AfterRender_Redirects_When_Null_Invoice()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns((Invoice)null);
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<AmendPaymentRequest>();

        navigationManager?.Uri.Should().Be("http://localhost/");
    }

    [Fact]
    public void Invoice_Values_Are_Displayed()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);

        var component = RenderComponent<AmendPaymentRequest>(parameters =>
            parameters.Add(p => p.PaymentRequestId, "1"));

        component.Instance.PaymentRequestId.Should().Be("1");

    }

    [Fact]
    public void AddInvoiceLine_Navigates_To_AddInvoiceLine()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);

        var component = RenderComponent<AmendPaymentRequest>(parameters =>
            parameters.Add(p => p.PaymentRequestId, "1"));

        var button = component.FindAll("button#add-invoice-line");

        button[0].Click();

        var navigationManager = Services.GetService<NavigationManager>();
        navigationManager?.Uri.Should().Be("http://localhost/invoice/add-invoice-line/1");
    }

    [Fact]
    public void UpdatePaymentRequest_Navigates_To_UpdatePaymentRequest()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);

        var component = RenderComponent<AmendPaymentRequest>(parameters =>
            parameters.Add(p => p.PaymentRequestId, "1"));

        var button = component.FindAll("button#update-payment-request");

        button[0].Click();

        var navigationManager = Services.GetService<NavigationManager>();
        navigationManager?.Uri.Should().Be("http://localhost/invoice/update-payment-request/1");
    }
}