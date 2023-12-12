using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Shared.Components.ApprovalCard;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Components;

public class ApprovalCardTests : TestContext
{
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    private readonly Invoice _invoice;
    public ApprovalCardTests()
    {
        _invoice = new Invoice()
        {
            PaymentRequests = new List<PaymentRequest>() { new PaymentRequest() }
        };

        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();
        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
    }

    [Fact]
    public void Nothing_Displayed_When_Invoice_Not_Set()
    {
        var component = RenderComponent<ApprovalCard>();
        component.Markup.Should().Contain("<h4 class=\"govuk-heading-s\">Nothing to Display</h4>");
    }

    [Fact]
    public void Parameters_Are_Set()
    {
        var component = RenderComponent<ApprovalCard>(parameters =>
        {
            parameters.Add(x => x.invoice, _invoice);
        });

        component.Instance.invoice.Should().NotBeNull();
        component.Instance.invoice.Should().BeOfType<Invoice>();
    }

    [Fact]
    public void Approve_Is_Selected()
    {
        var component = RenderComponent<ApprovalCard>(parameters =>
        {
            parameters.Add(x => x.invoice, _invoice);
        });

        component.FindAll("a#approve-approval-link")[0].Click();

        var navigationManager = Services.GetService<NavigationManager>();

        _mockInvoiceStateContainer.Verify(x => x.SetValue(It.IsAny<Invoice>()), Times.Once);
        component.WaitForAssertion(() => navigationManager?.Uri.Should().EndWith("/approval/confirm/approve"));


    }

    [Fact]
    public void Reject_Is_Selected()
    {
        var component = RenderComponent<ApprovalCard>(parameters =>
        {
            parameters.Add(x => x.invoice, _invoice);
        });

        component.FindAll("a#reject-approval-link")[0].Click();

        var navigationManager = Services.GetService<NavigationManager>();

        _mockInvoiceStateContainer.Verify(x => x.SetValue(It.IsAny<Invoice>()), Times.Once);
        component.WaitForAssertion(() => navigationManager?.Uri.Should().EndWith("/approval/confirm/reject"));

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

        var component = RenderComponent<ApprovalCard>(parameters =>
        {
            parameters.Add(x => x.invoice, invoice);
        });

        //Act
        var TotalValueGBP = component.FindAll("dd.govuk-summary-list__value")[4];
        var TotalValueEUR = component.FindAll("dd.govuk-summary-list__value")[5];
        // TODO: Remove index-based selectors in tests, as this is prone to errors and considered bad practice.

        //Assert
        Assert.Equal("336.01 GBP", TotalValueGBP.InnerHtml);
        Assert.Equal("557.67 EUR", TotalValueEUR.InnerHtml);
    }
}