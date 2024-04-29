using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.invoice.ViewInvoiceSummary;
using EST.MIT.Web.Shared;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

public class ViewInvoiceSummaryTests : TestContext
{
    private readonly Invoice _invoice;
    private readonly Mock<IInvoiceAPI> _mockApiService;

    public ViewInvoiceSummaryTests()
    {
        _invoice = new Invoice();
        _invoice.PaymentRequests.Add(new PaymentRequest()
        {
            FRN = "1234567890",
            SourceSystem = "",
            MarketingYear = "0",
            PaymentRequestNumber = 0,
            AgreementNumber = "",
            Value = 4.54M,
            DueDate = "",
            Currency = "GBP",
            InvoiceLines = new List<InvoiceLine>()
        });

        _invoice.PaymentRequests[0].InvoiceLines.Add(new InvoiceLine()
        {
            Value = 4.50M,
            DeliveryBody = "RP00",
            SchemeCode = "BPS",
            Description = "G00 - Gross Value"
        });


        _mockApiService = new Mock<IInvoiceAPI>();
        Services.AddSingleton<IInvoiceAPI>(_mockApiService.Object);
        Services.AddSingleton<IInvoiceStateContainer, InvoiceStateContainer>();
    }

    [Fact]
    public void PaymentRequests_Are_Displayed()
    {
        _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_invoice);

        var component = RenderComponent<ViewInvoiceSummary>(parameters =>
            parameters.Add(p => p.invoiceId, _invoice.Id.ToString()));

        var paymentRequests = component.FindAll("div.mit-payment-request-summary");

        paymentRequests.Should().NotBeEmpty();
        paymentRequests.Should().HaveCount(1);

    }

    [Fact]
    public void No_PaymentRequests_Warning_Is_Displayed_When_PaymentRequest_Count_Is_Zero()
    {
        _invoice.PaymentRequests = new List<PaymentRequest>();

        _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_invoice);

        var component = RenderComponent<ViewInvoiceSummary>(parameters =>
            parameters.Add(p => p.invoiceId, _invoice.Id.ToString()));

        var warning = component.FindAll("div#no-payment-request-warning");

        warning.Should().NotBeEmpty();
        warning.Should().HaveCount(1);
    }

    [Fact]
    public void AddPaymentRequest_Navigates_To_Add_PaymentRequest_Page()
    {
        _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_invoice);

        var component = RenderComponent<ViewInvoiceSummary>();
        var button = component.FindAll("button#add-payment-request");

        button[0].Click();

        var navigationManager = Services.GetService<NavigationManager>();
        component.WaitForAssertion(() =>
            navigationManager?.Uri.Should().Be("http://localhost/invoice/add-payment-request")
        );
    }

    [Fact]
    public void SendForApproval_Navigates_To_Select_Approval_Page()
    {
        _invoice.PaymentRequests[0].InvoiceLines.Add(new InvoiceLine()
        {
            Value = 34.67M,
            DeliveryBody = "RP00",
            SchemeCode = "BPS",
            Description = "G00 - Gross Value"
        });

        _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_invoice);

        var component = RenderComponent<ViewInvoiceSummary>(parameters =>
            parameters.Add(p => p.invoiceId, _invoice.Id.ToString()));

        component.FindAll("button#send-approval")[0].Click();

        var navigationManager = Services.GetService<NavigationManager>();
        component.WaitForAssertion(() =>
            navigationManager?.Uri.Should().Be("http://localhost/approval/select")
        );
    }

    [Fact]
    public void When_Invoice_Has_PaymentRequest_With_No_Invoicelines_Then_SendForApproval_Button_Is_Disabled()
    {
        //Arrange
        var invoice = new Invoice()
        {
            PaymentRequests = new List<PaymentRequest>()
            {
                new PaymentRequest()
                {
                    FRN = "1234567890",
                    SourceSystem = "",
                    MarketingYear = "0",
                    PaymentRequestNumber = 0,
                    AgreementNumber = "",
                    Value = 0,
                    DueDate = "",
                    Currency = "GBP"
                }
            }
        };
        _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(invoice);

        var component = RenderComponent<ViewInvoiceSummary>();

        //Act
        var value = component.FindAll("button#send-approval");

        //Assert
        value.Should().BeEmpty();
        value.Should().HaveCount(0);
    }

    [Fact]
    public void When_Invoice_Has_PaymentRequest_With_At_Least_One_InvoiceLine_With_Value_Not_Zero_Then_SendForApproval_Button_Is_Enaabled()
    {
        //Arrange
        var invoice = new Invoice()
        {
            PaymentRequests = new List<PaymentRequest>()
            {
                new PaymentRequest()
                {
                    FRN = "1234567890",
                    SourceSystem = "",
                    MarketingYear = "0",
                    PaymentRequestNumber = 0,
                    AgreementNumber = "",
                    Value = 34.89M,
                    DueDate = "",
                    Currency = "GBP",
                    InvoiceLines = new List<InvoiceLine>()
                    {
                        new InvoiceLine()
                        {
                             Value = 34.89M
                        } 
                    }
                }
            }
        };

        _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(invoice);

        var component = RenderComponent<ViewInvoiceSummary>();

        //Act
        var value = component.FindAll("button#send-approval");
        var innerHtml = value[0].InnerHtml;

        //Assert
        value.Should().NotBeEmpty();
        value.Count.Should().Be(1);
        innerHtml.Should().Be("Send For Approval");
    }

    [Fact]
    public void When_Invoice_Has_PaymentRequest_With__InvoiceLines_With_Total_Value_Equals_Zero_Then_SendForApproval_Button_Is_Disabled()
    {
        //Arrange
        var invoice = new Invoice()
        {
            PaymentRequests = new List<PaymentRequest>()
            {
                new PaymentRequest()
                {
                    FRN = "1234567890",
                    SourceSystem = "",
                    MarketingYear = "0",
                    PaymentRequestNumber = 0,
                    AgreementNumber = "",
                    Value = 0,
                    DueDate = "",
                    Currency = "GBP",
                    InvoiceLines = new List<InvoiceLine>()
                    {
                        new InvoiceLine()
                        {
                            Value = 0.00M
                        }
                    }
                }
            }
        };

        _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(invoice);

        var component = RenderComponent<ViewInvoiceSummary>();

        //Act
        var value = component.FindAll("button#send-approval");

        //Assert
        value.Should().BeEmpty();
        value.Should().HaveCount(0);
    }

    [Fact]
    public void When_Invoice_Has_PaymentRequests_With_InvoiceLines_Values_Then_TotalValues_Is_Displayed()
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
                }
            }
        };

        _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(invoice);

        var component = RenderComponent<ViewInvoiceSummary>();

        //Act
        var value = component.FindAll("dd#total-value-of-payments-gbp");
        var innerHtml = value[0].InnerHtml;

        //Assert
        value.Should().NotBeEmpty();
        value.Count.Should().Be(1);
        innerHtml.Should().Be("336.01 GBP");
    }
}