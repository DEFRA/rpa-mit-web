using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Pages.invoice.Summary;
using EST.MIT.Web.Shared.Components.UserInvoicesCard;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace EST.MIT.Web.Test.Components
{
    public class UserInvoicesCardTests : TestContext
    {
        private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
        private readonly Mock<IInvoiceAPI> _mockApiService;
        private readonly Invoice _invoice;

        private string backUrl = "/user-invoices";
        public UserInvoicesCardTests()
        {
            _invoice = new Invoice()
            {
                PaymentRequests = new List<PaymentRequest>() { new PaymentRequest() }
            };

            _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();
            Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
            _mockApiService = new Mock<IInvoiceAPI>();
            Services.AddSingleton<IInvoiceAPI>(_mockApiService.Object);
        }

        [Fact]
        public void Nothing_Displayed_When_Invoice_Not_Set()
        {
            var component = RenderComponent<UserInvoicesCard>();
            component.Markup.Should().Contain("<h4 class=\"govuk-heading-s\">Nothing to Display</h4>");
        }

        [Fact]
        public void Parameters_Are_Set()
        {
            var component = RenderComponent<UserInvoicesCard>(parameters =>
            {
                parameters.Add(x => x.invoice, _invoice);
            });

            component.Instance.invoice.Should().NotBeNull();
            component.Instance.invoice.Should().BeOfType<Invoice>();
        }

        [Fact]
        public void When_View_Link_Is_Click_InvoiceSummary_Page_Is_Display()
        {
            //Arrange
            var component = RenderComponent<UserInvoicesCard>(parameters =>
            {
                parameters.Add(x => x.invoice, _invoice);
            });

            component.FindAll("a.govuk-link")[0].Click();
            var navigationManager = Services.GetService<NavigationManager>();

            //Assert
            navigationManager?.Uri.Should().Be($"http://localhost/invoice/summary/{_invoice.SchemeType}/{_invoice.Id}/{WebUtility.UrlEncode(backUrl)}");

        }

        [Fact]
        public void When_Back_Link_IsClicked_On_InvoiceSummary_Page_Then_MyInvoices_Page_Is_Display()
        {
            //Arrange
            var summaryComponent = RenderComponent<Summary>(parameters =>
            {
                parameters.Add(x => x.backUrl, backUrl);
            });

            summaryComponent.FindAll("a.govuk-back-link")[0].GetAttribute("Back");

            var summaryNavigationManager = Services.GetService<NavigationManager>();

            //Assert
            summaryNavigationManager?.Uri.Should().Be($"http://localhost/{WebUtility.UrlEncode(backUrl)}");
        }
    }
}
