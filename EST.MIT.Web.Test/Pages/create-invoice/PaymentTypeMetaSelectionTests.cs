using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Pages.create_invoice.PaymentTypeMetaSelection;
using EST.MIT.Web.Shared;
using Services;
using Entities;
using System.Net;

namespace Pages.Tests;

public class PaymentTypeMetaSelectionPageTests : TestContext
{
    private readonly Mock<IPageServices> _mockPageServices;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    private readonly Mock<IReferenceDataAPI> _mockReferenceDataAPI;

    public PaymentTypeMetaSelectionPageTests()
    {
        _mockPageServices = new Mock<IPageServices>();
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();
        _mockReferenceDataAPI = new Mock<IReferenceDataAPI>();

        Services.AddSingleton<IReferenceDataAPI>(_mockReferenceDataAPI.Object);
        Services.AddSingleton<IPageServices>(_mockPageServices.Object);
        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
    }

    [Fact]
    public void AfterRender_Redirects_When_Null_Invoice()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns((Invoice)null);

        _mockReferenceDataAPI.Setup(x => x.GetPaymentTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns(Task.FromResult<ApiResponse<IEnumerable<PaymentScheme>>>(new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK)
        {
            Data = new List<PaymentScheme>
            {
               new PaymentScheme { code = "EU", description = "EU" }
            }
        }));

        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<PaymentTypeMetaSelection>();

        navigationManager?.Uri.Should().Be("http://localhost/create-invoice");
    }

    // [Fact]
    // public void No_Selection_Fails_Validation()
    // {
    //     _mockPageServices.Setup(x => x.Validation(It.IsAny<PaymentTypeSelect>(), out It.Ref<bool>.IsAny, out It.Ref<Dictionary<string, string>>.IsAny))
    //         .Callback((object paymentTypeSelect, out bool IsErrored, out Dictionary<string, string> errors) =>
    //         {
    //             IsErrored = true;
    //             errors = new()
    //             {
    //                 { "Name", "Please select a payment type" }
    //             };
    //         });

    //     _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());

    //     var component = RenderComponent<PaymentTypeMetaSelection>();
    //     component.FindAll("button")[0].Click();

    //     component.WaitForElements("p.govuk-error-message");

    //     var errorMessages = component.FindAll("p.govuk-error-message");

    //     var validation = Services.GetService<IPageServices>();

    //     errorMessages.Should().NotBeEmpty();
    //     errorMessages.Should().HaveCount(1);
    //     errorMessages[0].TextContent.Should().Be("Error:Please select a payment type");

    // }

    [Fact]
    public void Shows_PaymentType_RadioButtons()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());

        _mockReferenceDataAPI.Setup(x => x.GetPaymentTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns(Task.FromResult<ApiResponse<IEnumerable<PaymentScheme>>>(new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK)
        {
            Data = new List<PaymentScheme>
            {
               new PaymentScheme { code = "EU", description = "EU" }
            }
        }));

        var component = RenderComponent<PaymentTypeMetaSelection>();
        component.WaitForElements("input[type='radio']");
        var radioButtons = component.FindAll("input[type='radio']");

        radioButtons.Should().NotBeEmpty();
        radioButtons.Should().HaveCount(1);
        radioButtons[0].GetAttribute("value").Should().Be("EU");
    }

    [Fact]
    public void Saves_Selected_PaymentType_Navigates_To_Review()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());
        var navigationManager = Services.GetService<NavigationManager>();

        _mockReferenceDataAPI.Setup(x => x.GetPaymentTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns(Task.FromResult<ApiResponse<IEnumerable<PaymentScheme>>>(new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK)
        {
            Data = new List<PaymentScheme>
            { 
              new PaymentScheme { code = "EU", description = "EU"},
              new PaymentScheme { code = "DOMESTIC", description = "DOMESTIC" }
            }
        }));

        var component = RenderComponent<PaymentTypeMetaSelection>();
        var selectPaymentTypeRadioButton = component.FindAll("input[type='radio'][value='EU']");
        var saveAndContinueButton = component.FindAll("button[type='submit']");

        selectPaymentTypeRadioButton[0].Change("EU");
        saveAndContinueButton[0].Click();

        navigationManager?.Uri.Should().Be("http://localhost/create-invoice/review");
    }

    [Fact]
    public void Cancels_Invoice_Navigates_To_HomePage()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());
        var navigationManager = Services.GetService<NavigationManager>();

        _mockReferenceDataAPI.Setup(x => x.GetPaymentTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns(Task.FromResult<ApiResponse<IEnumerable<PaymentScheme>>>(new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK)
        {
            Data = new List<PaymentScheme>
            {
               new PaymentScheme { code = "EU", description = "EU" }
            }
        }));

        var component = RenderComponent<PaymentTypeMetaSelection>();
        var cancelButton = component.FindAll("a.govuk-link");

        cancelButton[0].Click();

        navigationManager?.Uri.Should().Be("http://localhost/");
    }
}
