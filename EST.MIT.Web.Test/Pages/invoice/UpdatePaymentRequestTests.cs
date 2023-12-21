using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.invoice.UpdatePaymentRequest;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

public class UpdatePaymentRequestTests : TestContext
{
    private readonly Invoice _invoice;
    private readonly Mock<IInvoiceAPI> _mockApiService;
    private readonly Mock<IPageServices> _mockPageServices;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    public UpdatePaymentRequestTests()
    {
        _invoice = new Invoice();
        _invoice.PaymentRequests.Add(new PaymentRequest()
        {
            PaymentRequestId = "1",
            FRN = "1234567890",
            SourceSystem = "Manual",
            MarketingYear = "2020",
            PaymentRequestNumber = 1,
            AgreementNumber = "123",
            Value = 123.45M,
            DueDate = "24/03/1990",
            InvoiceLines = new List<InvoiceLine>(),
            Currency = "GBP"
        });

        _invoice.PaymentRequests[0].InvoiceLines.Add(new InvoiceLine()
        {
            Value = 0,
            DeliveryBody = "RP00",
            SchemeCode = "",
            Description = ""
        });

        _mockApiService = new Mock<IInvoiceAPI>();
        _mockPageServices = new Mock<IPageServices>();
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();

        Services.AddSingleton<IInvoiceAPI>(_mockApiService.Object);
        Services.AddSingleton<IPageServices>(_mockPageServices.Object);
        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);

    }

    [Fact]
    public void Parameters_Are_Set()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);

        var component = RenderComponent<UpdatePaymentRequest>(parameters =>
            parameters.Add(p => p.PaymentRequestId, "1"));

        component.Instance.PaymentRequestId.Should().Be("1");
    }

    [Fact]
    public void SavePaymentRequest_Returns_To_Amend()
    {
        var IsErrored = false;
        var Errors = new Dictionary<string, List<string>>();

        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<PaymentRequest>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockPageServices.Setup(x => x.Validation(It.IsAny<PaymentRequest>(), out IsErrored, out Errors)).Returns(true);
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);

        var component = RenderComponent<UpdatePaymentRequest>(parameters =>
            parameters.Add(p => p.PaymentRequestId, "1"));

        var button = component.FindAll("button.govuk-button");

        button[0].Click();

        var navigationManager = Services.GetService<NavigationManager>();
        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be($"http://localhost/invoice/amend-payment-request/{component.Instance.PaymentRequestId}"));
    }

    [Fact]
    public void SavePaymentRequest_Navigates_To_Amend_On_Cancel()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);

        var component = RenderComponent<UpdatePaymentRequest>(parameters =>
            parameters.Add(p => p.PaymentRequestId, "1"));

        var link = component.FindAll("a.govuk-link");

        link[0].Click();

        var navigationManager = Services.GetService<NavigationManager>();
        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be($"http://localhost/invoice/amend-payment-request/{component.Instance.PaymentRequestId}"));
    }

    [Fact]
    public void ComponentRendersWithRequiredFields()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);

        var component = RenderComponent<UpdatePaymentRequest>(parameters =>
            parameters.Add(p => p.PaymentRequestId, "1"));

        Assert.NotNull(component.FindAll("input#frn")[0]);
        Assert.NotNull(component.FindAll("input#sbi")[0]);
        Assert.NotNull(component.FindAll("input#vendor")[0]);
        Assert.NotNull(component.FindAll("input#marketingyear")[0]);
        Assert.NotNull(component.FindAll("input#agreementnumber")[0]);
        Assert.NotNull(component.FindAll("select#currency")[0]);
        Assert.NotNull(component.FindAll("input#description")[0]);
    }
}