using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Shared.Components.InvoiceCard;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Components;

public class UserInvoicesCardTests : TestContext
{
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    private readonly Invoice _invoice;

    public UserInvoicesCardTests()
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
}
