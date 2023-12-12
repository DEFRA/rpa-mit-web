using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.invoice.AmendPaymentRequest;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

public class AmendPaymentRequestTests : TestContext
{
    private readonly Invoice _invoice;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    private readonly Mock<IInvoiceAPI> _mockInvoiceApi;

    public AmendPaymentRequestTests()
    {
        _invoice = new Invoice();
        _invoice.PaymentRequests.Add(new PaymentRequest()
        {
            PaymentRequestId = "1",
            FRN = "1234567890",
            SourceSystem = "",
            MarketingYear = "0",
            PaymentRequestNumber = 0,
            AgreementNumber = "",
            Value = 0,
            DueDate = "",
            Currency = "GBP",
            InvoiceLines = new List<InvoiceLine>()
        });

        _invoice.PaymentRequests[0].InvoiceLines.Add(new InvoiceLine()
        {
            Value = 100.00M,
            DeliveryBody = "RP00",
            SchemeCode = "BPS",
            Description = "G00 - Gross Value"
        });

        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();
        _mockInvoiceApi = new Mock<IInvoiceAPI>();
        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
        Services.AddSingleton<IInvoiceAPI>(_mockInvoiceApi.Object);
    }

    [Fact]
    public void AfterRender_Redirects_When_Null_Invoice()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns((Invoice?)null);
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<AmendPaymentRequest>();

        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be("http://localhost/"));
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
        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be("http://localhost/invoice/add-invoice-line/1"));
    }

    [Fact]
    public void AmendPaymentRequest_Navigates_To_UpdatePaymentRequest()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);

        var component = RenderComponent<AmendPaymentRequest>(parameters =>
            parameters.Add(p => p.PaymentRequestId, "1"));

        var button = component.FindAll("a#update-payment-request");

        button[0].Click();

        var navigationManager = Services.GetService<NavigationManager>();
        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be("http://localhost/invoice/update-payment-request/1"));
    }
}