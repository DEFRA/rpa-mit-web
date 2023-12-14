using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Pages.create_invoice.OrganisationMetaSelection;
using EST.MIT.Web.Entities;
using System.Net;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

public class OrganisationMetaSelectionPageTests : TestContext
{
    private readonly Mock<IReferenceDataAPI> _mockReferenceDataAPI;
    private readonly Mock<IPageServices> _mockPageServices;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;

    public OrganisationMetaSelectionPageTests()
    {
        _mockReferenceDataAPI = new Mock<IReferenceDataAPI>();
        _mockPageServices = new Mock<IPageServices>();
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();

        Services.AddSingleton<IReferenceDataAPI>(_mockReferenceDataAPI.Object);
        Services.AddSingleton<IPageServices>(_mockPageServices.Object);
        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
    }

    [Fact]
    public void AfterRender_Redirects_When_Null_Invoice()
    {
        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns((Invoice?)null);
        var navigationManager = Services.GetService<NavigationManager>();

        var component = RenderComponent<OrganisationMetaSelectionInvoice>();

        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be("http://localhost/create-invoice"));
    }

    [Fact]
    public void Shows_Organisation_RadioButtons()
    {

        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());

        _mockReferenceDataAPI.Setup(x => x.GetOrganisationsAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<ApiResponse<IEnumerable<Organisation>>>(new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK)
            {
                Data = new List<Organisation>
                {
                    new Organisation { code = "RPA", description = "Rural Payments Agency" }
                }
            }));

        var component = RenderComponent<OrganisationMetaSelectionInvoice>();
        component.WaitForElements("input[type='radio']");
        var radioButtons = component.FindAll("input[type='radio']");

        radioButtons.Should().NotBeEmpty();
        radioButtons.Should().HaveCount(1);
        radioButtons[0].GetAttribute("value").Should().Be("RPA");
    }

    [Fact]
    public void Saves_Selected_Organisation_Navigates_To_Scheme()
    {
        _mockInvoiceStateContainer.Setup(x => x.Value).Returns(new Invoice { AccountType = "RPA" });
        var navigationManager = Services.GetService<NavigationManager>();

        _mockReferenceDataAPI.Setup(x => x.GetOrganisationsAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<ApiResponse<IEnumerable<Organisation>>>(new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK)
            {
                Data = new List<Organisation>
                {
                    new Organisation { code = "RPA", description = "RPA" },
                    new Organisation { code = "FC", description = "FC" },
                    new Organisation { code = "NE", description = "NE" }
                }
            }));


        var component = RenderComponent<OrganisationMetaSelectionInvoice>();
        component.WaitForElements("input[type='radio']");
        var selectOrganisationRadioButton = component.FindAll("input[type='radio'][value='RPA']");
        var saveAndContinueButton = component.FindAll("button[type='submit']");

        selectOrganisationRadioButton[0].Change("RPA");
        saveAndContinueButton[0].Click();

        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be("http://localhost/create-invoice/scheme"));
    }

    [Fact]
    public void Cancels_Invoice_Navigates_To_HomePage()
    {
        _mockInvoiceStateContainer.Setup(x => x.Value).Returns(new Invoice { AccountType = "RPA" });
        var navigationManager = Services.GetService<NavigationManager>();

        _mockReferenceDataAPI.Setup(x => x.GetOrganisationsAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<ApiResponse<IEnumerable<Organisation>>>(new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK)
            {
                Data = new List<Organisation>
                {
                    new Organisation { code = "RPA", description = "RPA" },
                    new Organisation { code = "FC", description = "FC" },
                    new Organisation { code = "NE", description = "NE" }
                }
            }));


        var component = RenderComponent<OrganisationMetaSelectionInvoice>();
        component.WaitForElements("a.govuk-link");
        var cancelButton = component.FindAll("a.govuk-link");

        cancelButton[0].Click();

        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be("http://localhost/"));
    }
}