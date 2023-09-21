using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Entities;
using EST.MIT.Web.Pages.invoice.AddPaymentRequest;
using EST.MIT.Web.Shared;
using Services;

namespace Pages.Tests;

public class AddPaymentRequestTests : TestContext
{
    private readonly Invoice _invoice;
    private readonly Mock<IInvoiceAPI> _mockApiService;
    private readonly Mock<IPageServices> _mockPageServices;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;

    public AddPaymentRequestTests()
    {
        _invoice = new Invoice();
        _invoice.PaymentRequests.Add(new PaymentRequest()
        {
            CustomerId = "1234567890",
            Value = 0,
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
    public void Summary_Parameters_Are_Set()
    {
        var component = RenderComponent<AddPaymentRequest>(parameters =>
            parameters.Add(p => p.invoice, _invoice));

        component.Instance.invoice.Should().NotBeNull();
        component.Instance.invoice.PaymentRequests.Count.Should().Be(1);
    }

    // [Fact]
    // public void AfterRender_Redirects_When_Null_Invoice()
    // {
    //     _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns((Invoice)null);
    //     var navigationManager = Services.GetService<NavigationManager>();

    //     var component = RenderComponent<AddPaymentRequest>();

    //     navigationManager?.Uri.Should().Be("http://localhost/");
    // }

    [Fact]
    public void SaveHeader_Navigates_To_Add_Summary()
    {
        var IsErrored = false;
        var Errors = new Dictionary<string, List<string>>();

        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<PaymentRequest>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockPageServices.Setup(x => x.Validation(It.IsAny<PaymentRequest>(), out IsErrored, out Errors)).Returns(true);

        var component = RenderComponent<AddPaymentRequest>(parameters =>
            parameters.Add(p => p.invoice, _invoice));

        component.FindAll("button.govuk-button")[0].Click();

        var navigationManager = Services.GetService<NavigationManager>();
        navigationManager?.Uri.Should().Be($"http://localhost/invoice/summary/{component.Instance.invoice.SchemeType}/{component.Instance.invoice.Id}");
    }

    [Fact]
    public void SaveHeader_Navigates_To_Summary_On_Cancel()
    {
        var component = RenderComponent<AddPaymentRequest>(parameters =>
        {
            parameters.Add(p => p.invoice, _invoice);
        });

        var link = component.FindAll("a.govuk-link");

        link[0].Click();

        var navigationManager = Services.GetService<NavigationManager>();
        navigationManager?.Uri.Should().Be($"http://localhost/invoice/summary/{component.Instance.invoice.SchemeType}/{component.Instance.invoice.Id}");
    }
}