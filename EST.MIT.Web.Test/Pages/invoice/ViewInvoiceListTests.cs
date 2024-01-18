using EST.MIT.Web.Entities;
using EST.MIT.Web.Shared;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Shared.Components.InvoiceCard;
using EST.MIT.Web.Pages.invoice.ViewInvoiceList;
using Microsoft.Identity.Web;
using AngleSharp;

namespace EST.MIT.Web.Tests.Pages;

public class ViewInvoiceListTests : TestContext
{
    private readonly Mock<IInvoiceAPI> _mockApiService;

    public ViewInvoiceListTests()
    {
        _mockApiService = new Mock<IInvoiceAPI>();
        Services.AddSingleton(_mockApiService.Object);
        Services.AddSingleton<IInvoiceStateContainer, InvoiceStateContainer>();

        var tokenApi = new Mock<ITokenAcquisition>();
        Services.AddSingleton(tokenApi.Object);

        var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        config.Setup(c => c.GetSection("MitWebApi").Value).Returns("api://test_id");
        Services.AddSingleton(config.Object);

        var serviceProvider = new Mock<IServiceProvider>();
        var consentHandler = new Mock<MicrosoftIdentityConsentAndConditionalAccessHandler>(serviceProvider.Object);
        Services.AddSingleton(consentHandler.Object);
    }

    [Fact]
    public void UserInvoices_Renders()
    {
        var component = RenderComponent<ViewInvoiceList>();
        component.Should().NotBeNull();
    }

    [Fact]
    public void UserInvoices_Lists_UserInvoiceCards()
    {
        var invoices = new List<Invoice>()
        {
            {
                new Invoice()
                {
                    PaymentRequests = new List<PaymentRequest>()
                    {
                        new PaymentRequest()
                    }
                }
            }
        };

        _mockApiService.Setup(x => x.GetInvoicesAsync(null)).ReturnsAsync(invoices);

        var component = RenderComponent<ViewInvoiceList>();
        component.WaitForElements("div.govuk-summary-card");

        component.FindComponents<InvoiceCard>().Count.Should().Be(1);
    }
}