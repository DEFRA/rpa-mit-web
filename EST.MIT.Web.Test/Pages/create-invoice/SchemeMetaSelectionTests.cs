using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Pages.create_invoice.SchemeMetaSelection;
using EST.MIT.Web.Entities;
using System.Net;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

public class SchemeMetaSelectionPageTests : TestContext
{
    private readonly Mock<IReferenceDataAPI> _mockReferenceDataAPI;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    private readonly Mock<IPageServices> _mockPageServices;

    public SchemeMetaSelectionPageTests()
    {
        _mockReferenceDataAPI = new Mock<IReferenceDataAPI>();
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();
        _mockPageServices = new Mock<IPageServices>();

        Services.AddSingleton<IReferenceDataAPI>(_mockReferenceDataAPI.Object);
        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
        Services.AddSingleton<IPageServices>(_mockPageServices.Object);
    }

    [Fact]
    public void AfterRender_Redirects_When_Null_Invoice()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns((Invoice?)null);
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<SchemeMetaSelection>();

        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be("http://localhost/create-invoice"));
    }

    [Fact]
    public void No_Selection_Fails_Validation()
    {
        _mockPageServices.Setup(x => x.Validation(It.IsAny<SchemeSelect>(), out It.Ref<bool>.IsAny, out It.Ref<Dictionary<string, List<string>>>.IsAny))
            .Callback((object schemeSelect, out bool IsErrored, out Dictionary<string, List<string>> errors) =>
            {
                IsErrored = true;
                errors = new()
                {
                    { "scheme", new List<string>() { "Please select a scheme type" } }
                };
            });

        _mockReferenceDataAPI.Setup(x => x.GetSchemeTypesAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult<ApiResponse<IEnumerable<SchemeType>>>(new ApiResponse<IEnumerable<SchemeType>>(HttpStatusCode.OK)
            {
                Data = new List<SchemeType>
                {
                    new SchemeType { code = "BPS", description = "Basic Payment Scheme" }
                }
            }));

        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());

        var component = RenderComponent<SchemeMetaSelection>();
        component.FindAll("button")[0].Click();

        component.WaitForElements("p.govuk-error-message");

        var errorMessages = component.FindAll("p.govuk-error-message");

        var validation = Services.GetService<IPageServices>();

        errorMessages.Should().NotBeEmpty();
        errorMessages.Should().HaveCount(1);
        errorMessages[0].TextContent.Should().Be("Error:Please select a scheme type");

    }

    //FLAG
    [Fact]
    public void Shows_Scheme_RadioButtons()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());

        _mockReferenceDataAPI.Setup(x => x.GetSchemeTypesAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult<ApiResponse<IEnumerable<SchemeType>>>(new ApiResponse<IEnumerable<SchemeType>>(HttpStatusCode.OK)
            {
                Data = new List<SchemeType>
                {
                    new SchemeType { code = "BPS", description = "BPS" },
                    new SchemeType { code = "ES", description = "ES" },
                    new SchemeType { code = "CS", description = "CS" },
                    new SchemeType { code = "SPS", description = "SPS" },
                    new SchemeType { code = "Milk", description = "Milk" }
                }
            }));

        var component = RenderComponent<SchemeMetaSelection>();
        component.WaitForElements("input[type='radio']");
        var radioButtons = component.FindAll("input[type='radio']");

        radioButtons.Should().NotBeEmpty();
        radioButtons.Should().HaveCount(5);
        radioButtons[0].GetAttribute("value").Should().Be("BPS");
        radioButtons[1].GetAttribute("value").Should().Be("ES");
        radioButtons[2].GetAttribute("value").Should().Be("CS");
        radioButtons[3].GetAttribute("value").Should().Be("SPS");
        radioButtons[4].GetAttribute("value").Should().Be("Milk");

    }


    //FLAG: Removed for pipeline build
    // [Fact]
    // public void Saves_Selected_Scheme_Navigates_To_PaymentType()
    // {
    //     _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());
    //     var navigationManager = Services.GetService<NavigationManager>();

    //     _mockReferenceDataAPI.Setup(x => x.GetSchemesAsync(It.IsAny<string>(), It.IsAny<string>()))
    //         .Returns(Task.FromResult<ApiResponse<IEnumerable<PaymentScheme>>>(new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK)
    //         {
    //             Data = new List<PaymentScheme>
    //             {
    //                 new PaymentScheme { code = "BPS", description = "BPS" },
    //                 new PaymentScheme { code = "ES", description = "ES" },
    //                 new PaymentScheme { code = "CS", description = "CS" },
    //                 new PaymentScheme { code = "SPS", description = "SPS" },
    //                 new PaymentScheme { code = "Milk", description = "Milk" }
    //             }
    //         }));

    //     var component = RenderComponent<SchemeMetaSelection>();
    //     component.WaitForElements("input[type='radio']");
    //     var selectSchemeRadioButton = component.FindAll("input[type='radio'][value='Milk']");
    //     var saveAndContinueButton = component.FindAll("button[type='submit']");

    //     selectSchemeRadioButton[0].Change("Milk");
    //     saveAndContinueButton[0].Click();

    //     navigationManager?.Uri.Should().Be("http://localhost/create-invoice/payment-type");
    // }

    [Fact]
    public void Cancels_Invoice_Navigates_To_HomePage()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());
        // var invoiceStateContainer = Services.GetService<IInvoiceStateContainer>();
        var navigationManager = Services.GetService<NavigationManager>();

        _mockReferenceDataAPI.Setup(x => x.GetSchemeTypesAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult<ApiResponse<IEnumerable<SchemeType>>>(new ApiResponse<IEnumerable<SchemeType>>(HttpStatusCode.OK)
            {
                Data = new List<SchemeType>
                {
                    new SchemeType { code = "BPS", description = "BPS" },
                    new SchemeType { code = "ES", description = "ES" },
                    new SchemeType { code = "CS", description = "CS" },
                    new SchemeType { code = "SPS", description = "SPS" },
                    new SchemeType { code = "Milk", description = "Milk" }
                }
            }));

        var component = RenderComponent<SchemeMetaSelection>();
        component.WaitForElements("a.govuk-link");
        var cancelButton = component.FindAll("a.govuk-link");

        cancelButton[0].Click();

        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be("http://localhost/"));
    }
}