using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Pages.create_invoice.AccountMetaSelection;
using EST.MIT.Web.Shared;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

public class AccountMetaSelectionPageTests : TestContext
{
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    private readonly Mock<IPageServices> _mockPageServices;
    public AccountMetaSelectionPageTests()
    {
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();
        _mockPageServices = new Mock<IPageServices>();

        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
        Services.AddSingleton<IPageServices>(_mockPageServices.Object);
    }

    [Fact]
    public void AfterRender_Redirects_When_Null_Invoice()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns((Invoice?)null);
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<AccountMetaSelection>();

        navigationManager?.Uri.Should().Be("http://localhost/create-invoice");
    }

    // [Fact]
    // public void No_Selection_Fails_Validation()
    // {

    //     _mockPageServices.Setup(x => x.Validation(It.IsAny<AccountSelect>(), out It.Ref<bool>.IsAny, out It.Ref<Dictionary<string, string>>.IsAny))
    //         .Callback((object accountSelect, out bool IsErrored, out Dictionary<string, string> errors) =>
    //         {
    //             IsErrored = true;
    //             errors = new()
    //             {
    //                 { "Name", "Please select an account type" }
    //             };
    //         });

    //     _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());

    //     var component = RenderComponent<AccountMetaSelection>();
    //     component.FindAll("button")[0].Click();

    //     component.WaitForElements("p.govuk-error-message");

    //     var errorMessages = component.FindAll("p.govuk-error-message");

    //     var validation = Services.GetService<IPageServices>();

    //     errorMessages.Should().NotBeEmpty();
    //     errorMessages.Should().HaveCount(1);
    //     errorMessages[0].TextContent.Should().Be("Error:Please select an account type");

    // }

    [Fact]
    public void Shows_AccountType_RadioButtons()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());

        var component = RenderComponent<AccountMetaSelection>();
        var radioButtons = component.FindAll("input[type='radio']");

        radioButtons.Should().NotBeEmpty();
        radioButtons.Should().HaveCount(2);
        radioButtons[0].GetAttribute("value").Should().Be("AR");
        radioButtons[1].GetAttribute("value").Should().Be("AP");
    }

    [Fact]
    public void Saves_Selected_AccountType_Navigates_To_Organisation()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());
        var invoiceStateContainer = Services.GetService<IInvoiceStateContainer>();
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<AccountMetaSelection>();
        var selectAccountTypeRadioButton = component.FindAll("input[type='radio'][value='AR']");
        var saveAndContinueButton = component.FindAll("button[type='submit']");

        selectAccountTypeRadioButton[0].Change("AR");
        saveAndContinueButton[0].Click();

        invoiceStateContainer?.Value.Should().NotBeNull();
        invoiceStateContainer?.Value.AccountType.Should().Be("AR");
        navigationManager?.Uri.Should().Be("http://localhost/create-invoice/organisation");
    }

    [Fact]
    public void Cancels_Invoice_Navigates_To_HomePage()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());
        var invoiceStateContainer = Services.GetService<IInvoiceStateContainer>();
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<AccountMetaSelection>();
        var cancelButton = component.FindAll("a.govuk-link");

        cancelButton[0].Click();

        invoiceStateContainer?.Value.Should().NotBeNull();
        invoiceStateContainer?.Value.AccountType.Should().BeNullOrEmpty();
        navigationManager?.Uri.Should().Be("http://localhost/");
    }
}