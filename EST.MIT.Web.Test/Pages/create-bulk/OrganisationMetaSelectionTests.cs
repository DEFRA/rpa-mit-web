using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Pages.create_bulk.OrganisationMetaSelection;
using EST.MIT.Web.Shared;
using EST.MIT.Web.Entities;
using System.Net;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

public class OrganisationMetaSelectionPageBulkTests : TestContext
{
    private readonly Mock<IReferenceDataAPI> _mockReferenceDataAPI;
    private readonly Mock<IPageServices> _mockPageServices;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;

    public OrganisationMetaSelectionPageBulkTests()
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

        var component = RenderComponent<OrganisationMetaSelection>();

        navigationManager?.Uri.Should().Be("http://localhost/create-bulk");
    }

    [Fact]
    public void No_Selection_Fails_Validation()
    {
        _mockPageServices.Setup(x => x.Validation(It.IsAny<OrganisationSelect>(), out It.Ref<bool>.IsAny, out It.Ref<Dictionary<string, List<string>>>.IsAny))
            .Callback((object organisationSelect, out bool IsErrored, out Dictionary<string, List<string>> errors) =>
            {
                IsErrored = true;
                errors = new()
                {
                    { "organisation", new List<string>() { "Please select an organisation" } }
                };
            });

        _mockReferenceDataAPI.Setup(x => x.GetOrganisationsAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<ApiResponse<IEnumerable<Organisation>>>(new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK)
            {
                Data = new List<Organisation>
                {
                     new Organisation { code = "RPA", description = "Rural Payments Agency" }
                }
            }));

        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());

        var component = RenderComponent<OrganisationMetaSelection>();
        component.FindAll("button")[0].Click();

        component.WaitForElements("p.govuk-error-message");

        var errorMessages = component.FindAll("p.govuk-error-message");

        var validation = Services.GetService<IPageServices>();

        errorMessages.Should().NotBeEmpty();
        errorMessages.Should().HaveCount(1);
        errorMessages[0].TextContent.Should().Be("Error:Please select an organisation");
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

        var component = RenderComponent<OrganisationMetaSelection>();
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

        var component = RenderComponent<OrganisationMetaSelection>();
        component.WaitForElements("input[type='radio']");
        var selectOrganisationRadioButton = component.FindAll("input[type='radio'][value='RPA']");
        var saveAndContinueButton = component.FindAll("button[type='submit']");

        selectOrganisationRadioButton[0].Change("RPA");
        saveAndContinueButton[0].Click();

        navigationManager?.Uri.Should().Be("http://localhost/create-bulk/scheme");
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


        var component = RenderComponent<OrganisationMetaSelection>();
        component.WaitForElements("a.govuk-link");
        var cancelButton = component.FindAll("a.govuk-link");

        cancelButton[0].Click();

        navigationManager?.Uri.Should().Be("http://localhost/");
    }
}