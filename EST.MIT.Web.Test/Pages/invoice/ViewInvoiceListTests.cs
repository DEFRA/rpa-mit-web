using EST.MIT.Web.Entities;
using EST.MIT.Web.Shared;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Shared.Components.InvoiceCard;
using EST.MIT.Web.Pages.invoice.ViewInvoiceList;
using Microsoft.Identity.Web;
using AngleSharp;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace EST.MIT.Web.Tests.Pages;

public class ViewInvoiceListTests : TestContext
{
    private readonly Mock<IInvoiceAPI> _mockApiService;
    private readonly Mock<ILogger<ViewInvoiceList>> _mockLogger;
    private readonly Mock<ITokenAcquisition> _tokenHandler;
    private readonly Mock<MicrosoftIdentityConsentAndConditionalAccessHandler> _consentHandler;

    public ViewInvoiceListTests()
    {
        _mockApiService = new Mock<IInvoiceAPI>();
        Services.AddSingleton(_mockApiService.Object);
        Services.AddSingleton<IInvoiceStateContainer, InvoiceStateContainer>();

        _mockApiService = new Mock<IInvoiceAPI>();
        Services.AddSingleton(_mockApiService.Object);

        _tokenHandler = new Mock<ITokenAcquisition>();
        Services.AddSingleton(_tokenHandler.Object);

        var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        config.Setup(c => c.GetSection("MitWebApi").Value).Returns("api://test_id");
        Services.AddSingleton(config.Object);

        var serviceProvider = new Mock<IServiceProvider>();
        _consentHandler = new Mock<MicrosoftIdentityConsentAndConditionalAccessHandler>(serviceProvider.Object);
        Services.AddSingleton(_consentHandler.Object);

        _mockLogger = new Mock<ILogger<ViewInvoiceList>>();
        Services.AddSingleton(_mockLogger.Object);
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

    [Fact]
    public void GetAccessTokenForUserAsync_NoToken_ThrowsException()
    {
        _tokenHandler.Setup(x => x.GetAccessTokenForUserAsync(new string[] { "api://test_id" }, null, null, null, null)).Throws(
            new MicrosoftIdentityWebChallengeUserException(
                new Microsoft.Identity.Client.MsalUiRequiredException("Test", "Test"), null, null));

        try
        {
            var component = RenderComponent<ViewInvoiceList>();
        }
        catch(Exception exc)
        {
            // The call to: ConsentHandler.HandleException(ex) throws.
        }

        _mockLogger.Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void GetInvoicesAsync_ApiError_ThrowsException()
    {
        _mockApiService.Setup(x => x.GetInvoicesAsync(null)).Throws(new Exception());

        RenderComponent<ViewInvoiceList>();

        _mockLogger.Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
    }
}