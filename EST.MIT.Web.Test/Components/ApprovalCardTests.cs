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

}