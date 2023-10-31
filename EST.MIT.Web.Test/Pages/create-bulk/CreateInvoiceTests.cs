using EST.MIT.Web.Pages.create_bulk.CreateBulk;
using EST.MIT.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace EST.MIT.Web.Tests.Pages;

public class CreateBulkTests : TestContext
{
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;

    public CreateBulkTests()
    {
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();

        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
    }

    [Fact]
    public void StartPage_Renders()
    {
        var component = RenderComponent<CreateBulk>();
        component.RenderCount.Should().Be(1);
    }

    [Fact]
    public void StartPage_StartButton_Navigates_To_AccountPage()
    {
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<CreateBulk>();
        component.FindAll("button.govuk-button")[0].Click();

        navigationManager?.Uri.Should().Be("http://localhost/create-bulk/account");
    }

    [Fact]
    public void StartPage_CancelButton_Navigates_To_HomePage()
    {
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<CreateBulk>();
        component.FindAll("a.govuk-link")[0].Click();

        navigationManager?.Uri.Should().Be("http://localhost/");
    }
}