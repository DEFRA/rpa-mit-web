using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Pages.create_invoice.PaymentTypeMetaSelection;
using EST.MIT.Web.Entities;
using System.Net;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

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
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns((Invoice?)null);

        _mockReferenceDataAPI.Setup(x => x.GetPaymentTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns(Task.FromResult<ApiResponse<IEnumerable<PaymentType>>>(new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK)
        {
            Data = new List<PaymentType>
            {
               new PaymentType { code = "EU", description = "EU" }
            }
        }));

        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<PaymentTypeMetaSelection>();

        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be("http://localhost/create-invoice"));
    }

    [Fact]
    public void Shows_PaymentType_RadioButtons()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());

        _mockReferenceDataAPI.Setup(x => x.GetPaymentTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns(Task.FromResult<ApiResponse<IEnumerable<PaymentType>>>(new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK)
        {
            Data = new List<PaymentType>
            {
               new PaymentType { code = "EU", description = "EU" }
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
    public void Saves_Selected_PaymentType_Navigates_To_Review_Invoice()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());
        var navigationManager = Services.GetService<NavigationManager>();

        _mockReferenceDataAPI.Setup(x => x.GetPaymentTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns(Task.FromResult<ApiResponse<IEnumerable<PaymentType>>>(new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK)
        {
            Data = new List<PaymentType>
            {
              new PaymentType { code = "EU", description = "EU"},
              new PaymentType { code = "DOMESTIC", description = "DOMESTIC" }
            }
        }));

        var component = RenderComponent<PaymentTypeMetaSelection>();
        component.WaitForElements("input[type='radio']");
        var selectPaymentTypeRadioButton = component.FindAll("input[type='radio'][value='EU']");
        var saveAndContinueButton = component.FindAll("button[type='submit']");

        selectPaymentTypeRadioButton[0].Change("EU");
        saveAndContinueButton[0].Click();

        component.WaitForState(() => component.Instance.IsDataLoaded);

        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be("http://localhost/create-invoice/review"));
    }

    [Fact]
    public void Cancels_Invoice_Navigates_To_HomePage()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());
        var navigationManager = Services.GetService<NavigationManager>();

        _mockReferenceDataAPI.Setup(x => x.GetPaymentTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns(Task.FromResult<ApiResponse<IEnumerable<PaymentType>>>(new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK)
        {
            Data = new List<PaymentType>
            {
               new PaymentType { code = "EU", description = "EU" }
            }
        }));

        var component = RenderComponent<PaymentTypeMetaSelection>();
        var cancelButton = component.FindAll("a.govuk-link");

        cancelButton[0].Click();

        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be("http://localhost/"));
    }
}