using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Pages.create_invoice.CreateInvoice;
using EST.MIT.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace EST.MIT.Web.Tests.Pages;

public class CreateInvoiceTests : TestContext
{
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;

    public CreateInvoiceTests()
    {
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();

        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
    }

    [Fact]
    public void StartPage_Renders()
    {
        var component = RenderComponent<CreateInvoice>();
        component.RenderCount.Should().Be(1);
    }

    [Fact]
    public void StartPage_StartButton_Navigates_To_AccountPage()
    {
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<CreateInvoice>();
        component.FindAll("button.govuk-button")[0].Click();

        navigationManager?.Uri.Should().Be("http://localhost/create-invoice/account");
    }

    [Fact]
    public void StartPage_CancelButton_Navigates_To_HomePage()
    {
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<CreateInvoice>();
        component.FindAll("a.govuk-link")[0].Click();

        navigationManager?.Uri.Should().Be("http://localhost/");
    }
}