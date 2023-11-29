using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.invoice.AddInvoiceLine;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

public class AddInvoiceLineTests : TestContext
{
    private readonly Invoice _invoice;
    private readonly Mock<IInvoiceAPI> _mockApiService;
    private readonly Mock<IPageServices> _mockPageServices;
    private readonly Mock<IReferenceDataAPI> _mockReferenceDataServices;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;

    public AddInvoiceLineTests()
    {
        _invoice = new Invoice()
        {
            SchemeType = "AC",
            Organisation = "RPA",
            PaymentType = "DOM",
            AccountType = "AR"
        };

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
        _mockReferenceDataServices = new Mock<IReferenceDataAPI>();
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();

        Services.AddSingleton<IInvoiceAPI>(_mockApiService.Object);
        Services.AddSingleton<IPageServices>(_mockPageServices.Object);
        Services.AddSingleton<IReferenceDataAPI>(_mockReferenceDataServices.Object);
        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
    }

    [Fact(Timeout = 100000)]
    public async void AfterRender_Redirects_When_Null_Invoice()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns((Invoice?)null);
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<AddInvoiceLine>();
        await Task.Delay(1000);

        navigationManager?.Uri.Should().Be("http://localhost/");
    }

    [Fact(Timeout = 100000)]
    public async void SaveInvoiceLine_Navigates_To_Add_AmendHeader()
    {
        var IsErrored = false;
        var Errors = new Dictionary<string, List<string>>();

        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<PaymentRequest>(), It.IsAny<InvoiceLine>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockPageServices.Setup(x => x.Validation(It.IsAny<InvoiceLine>(), out IsErrored, out Errors)).Returns(true);
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);

        var component = RenderComponent<AddInvoiceLine>(parameters =>
            parameters.Add(p => p.PaymentRequestId, "1"));
        await Task.Delay(1000);

        component.FindAll("button.govuk-button")[0].Click();

        var navigationManager = Services.GetService<NavigationManager>();
        navigationManager?.Uri.Should().Be($"http://localhost/invoice/amend-payment-request/1");
    }

    [Fact(Timeout = 100000)]
    public async void SaveInvoiceLine_Navigates_To_Add_AmendHeader_On_Cancel()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(_invoice);

        var component = RenderComponent<AddInvoiceLine>(parameters =>
            parameters.Add(p => p.PaymentRequestId, "1"));
        await Task.Delay(1000);

        component.FindAll("a.govuk-link")[0].Click();

        var navigationManager = Services.GetService<NavigationManager>();
        navigationManager?.Uri.Should().Be($"http://localhost/invoice/amend-payment-request/1");

    }
}