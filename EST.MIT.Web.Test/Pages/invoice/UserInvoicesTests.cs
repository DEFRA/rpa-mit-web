using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.invoice.UserInvoices;
using EST.MIT.Web.Shared;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Shared.Components.ApprovalCard;
using EST.MIT.Web.Shared.Components.UserInvoicesCard;

namespace EST.MIT.Web.Tests.Pages;

public class UserInvoicesTests : TestContext
{
    private readonly Mock<IInvoiceAPI> _mockApiService;

    public UserInvoicesTests()
    {
        _mockApiService = new Mock<IInvoiceAPI>();
        Services.AddSingleton<IInvoiceAPI>(_mockApiService.Object);
        Services.AddSingleton<IInvoiceStateContainer, InvoiceStateContainer>();
    }

    [Fact]
    public void UserInvoices_Renders()
    {
        var component = RenderComponent<UserInvoices>();
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

        _mockApiService.Setup(x => x.GetInvoicesAsync()).ReturnsAsync(invoices);

        var component = RenderComponent<UserInvoices>();
        component.WaitForElements("div.govuk-summary-card");

        component.FindComponents<UserInvoicesCard>().Count.Should().Be(1);
    }
}