using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Pages.invoice.ViewInvoiceSummary;
using EST.MIT.Web.Shared.Components.InvoiceCard;
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
            var component = RenderComponent<InvoiceCard>();
            component.Markup.Should().Contain("<h4 class=\"govuk-heading-s\">Nothing to Display</h4>");
        }

        [Fact]
        public void Parameters_Are_Set()
        {
            var component = RenderComponent<InvoiceCard>(parameters =>
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
            var component = RenderComponent<InvoiceCard>(parameters =>
            {
                parameters.Add(x => x.invoice, _invoice);
            });

            component.FindAll("a.govuk-link")[0].Click();
            var navigationManager = Services.GetService<NavigationManager>();

            //Assert
            navigationManager?.Uri.Should().Be($"http://localhost/invoice/summary/{_invoice.SchemeType}/{_invoice.Id}/{WebUtility.UrlEncode("/user-invoices")}");
        }

        [Fact]
        public void When_Invoice_Has_PaymentRequests_With_InvoiceLines_Values_Then_TotalValues_Plus_CurrencyType_Is_Displayed()
        {
            //Arrange
            var invoice = new Invoice()
            {
                Id = new Guid(),
                AccountType = "AR",
                Organisation = "NE",
                SchemeType = "CS",
                PaymentType = "GBP",
                PaymentRequests = new List<PaymentRequest>()
                {
                    new PaymentRequest()
                    {
                        FRN = "9999999987",
                        MarketingYear = "2023",
                        Currency = "GBP",
                        SBI = "1",
                        AgreementNumber = "EXT345",
                        Value = 30.67M,
                        InvoiceLines = new List<InvoiceLine>()
                        {
                                new InvoiceLine()
                                {
                                    Value = 30.67M
                                }
                        }
                    },
                    new PaymentRequest()
                    {
                        FRN = "4599999987",
                        MarketingYear = "2023",
                        Currency = "GBP",
                        SBI = "1",
                        AgreementNumber = "DE34",
                        Value = 305.34M,
                        InvoiceLines = new List<InvoiceLine>()
                        {
                                new InvoiceLine()
                                {
                                    Value = 305.34M
                                }
                        }
                    },
                    new PaymentRequest()
                    {
                        FRN = "9999999987",
                        MarketingYear = "2023",
                        Currency = "EUR",
                        SBI = "1",
                        AgreementNumber = "CC4",
                        Value = 555.67M,
                        InvoiceLines = new List<InvoiceLine>()
                        {
                                new InvoiceLine()
                                {
                                    Value = 555.67M
                                }
                        }
                    },
                    new PaymentRequest()
                    {
                        FRN = "9999999911",
                        MarketingYear = "2023",
                        Currency = "EUR",
                        SBI = "4",
                        AgreementNumber = "CD4",
                        Value = 2.00M,
                        InvoiceLines = new List<InvoiceLine>()
                        {
                                new InvoiceLine()
                                {
                                    Value = 2.00M
                                }
                        }
                    }
                }
            };

            var component = RenderComponent<InvoiceCard>(parameters =>
            {
                parameters.Add(x => x.invoice, invoice);
            });

            //Act
            var TotalValueGBP = component.FindAll("dd.govuk-summary-list__value")[3];
            var TotalValueEUR = component.FindAll("dd.govuk-summary-list__value")[4];
            var TotalPaymentRequests = component.FindAll("dd.govuk-summary-list__value")[2];
            // TODO: Remove index-based selectors in tests, as this is prone to errors and considered bad practice.

            //Assert
            Assert.Equal("336.01 GBP", TotalValueGBP.InnerHtml);
            Assert.Equal("557.67 EUR", TotalValueEUR.InnerHtml);
            Assert.Equal("4", TotalPaymentRequests.InnerHtml);
        }

        [Fact]
        public void Back_Link_On_InvoiceSummary_Page_Navigates_To_MyInvoices_Page_()
        {
            //Arrange
            var component = RenderComponent<ViewInvoiceSummary>(parameters =>
            {
                parameters.Add(x => x.backUrl, "/user-invoices");
            });

            var userInvoicesUrl = component.FindAll("a")[0].GetAttribute("href");

            //Assert
            Assert.Equal("/user-invoices", userInvoicesUrl);
        }
    }
}
