using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.find.Find;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

public class FindTests : TestContext
{
    private readonly Mock<IPageServices> _mockPageServices;
    private readonly Mock<IFindService> _mockFindService;
    private readonly Invoice _invoice;
    public FindTests()
    {
        _invoice = new Invoice()
        {
            Id = Guid.NewGuid(),
            SchemeType = "BPS",
        };

        _mockPageServices = new Mock<IPageServices>();
        _mockFindService = new Mock<IFindService>();
        Services.AddSingleton<IPageServices>(_mockPageServices.Object);
        Services.AddSingleton<IFindService>(_mockFindService.Object);
    }

    [Fact]
    public void Search_Navigates_To_Summary()
    {
        _mockFindService.Setup(x => x.FindInvoiceAsync(It.IsAny<SearchCriteria>())).ReturnsAsync(_invoice);
        _mockPageServices.Setup(x => x.Validation(It.IsAny<SearchCriteria>(), out It.Ref<bool>.IsAny, out It.Ref<Dictionary<string, List<string>>>.IsAny)).Returns(true);

        var component = RenderComponent<Find>();

        component.FindAll("input#invoicenumber")[0].Change(_invoice.Id.ToString());

        var button = component.FindAll("button.govuk-button")[0];
        button.Click();

        var navigationManager = Services.GetService<NavigationManager>();
        navigationManager?.Uri.Should().Be($"http://localhost/invoice/summary/{_invoice.SchemeType}/{_invoice.Id}");
    }

    [Fact]
    public void Search_Fields_Are_Required()
    {
        bool IsErrored = true;
        Dictionary<string, List<string>> Errors = new()
        {
            { "Search Criteria", new List<string>() { "An Invoice ID or Payment Requiest ID is required" } }
        };

        _mockPageServices.Setup(x => x.Validation(It.IsAny<SearchCriteria>(), out IsErrored, out Errors)).Returns(false);

        var component = RenderComponent<Find>();

        var button = component.FindAll("button.govuk-button")[0];
        button.Click();

        _mockPageServices.Verify(x => x.Validation(component.Instance._searchCriteria, out IsErrored, out Errors), Times.Once);

        component.WaitForElements("ul.govuk-error-summary__list");

        var errors = component.FindAll("ul.govuk-error-summary__list");

        errors.Should().NotBeNull();
        errors.Count.Should().Be(1);

        var errorItems = component.FindAll("ul.govuk-error-summary__list li");

        errorItems.Should().NotBeNull();
        errorItems.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void FindInvoice_ByInvoiceId_Not_Found()
    {
        var IsErrored = false;
        var Errors = new Dictionary<string, List<string>>();

        _mockFindService.Setup(x => x.FindInvoiceAsync(It.IsAny<SearchCriteria>())).Returns(Task.FromResult<Invoice>(null!));
        _mockPageServices.Setup(x => x.Validation(It.IsAny<SearchCriteria>(), out IsErrored, out Errors)).Returns(true);

        var component = RenderComponent<Find>();

        component.FindAll("input#invoicenumber")[0].Change("NotAnInvoiceId");

        var button = component.FindAll("button.govuk-button")[0];
        button.Click();

        component.WaitForElements("ul.govuk-error-summary__list");

        component.Instance.Invoice.Should().BeNull();

        var errors = component.FindAll("ul.govuk-error-summary__list");

        errors.Should().NotBeNull();
        errors.Count.Should().Be(1);

        var errorItems = component.FindAll("ul.govuk-error-summary__list li");

        errorItems.Should().NotBeNull();
        errorItems.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void FindInvoice_ByPaymentRequestId_Not_Found()
    {
        var IsErrored = true;
        var Errors = new Dictionary<string, List<string>>();

        _mockFindService.Setup(x => x.FindInvoiceAsync(It.IsAny<SearchCriteria>())).Returns(Task.FromResult<Invoice>(null!));
        _mockPageServices.Setup(x => x.Validation(It.IsAny<SearchCriteria>(), out IsErrored, out Errors)).Returns(true);

        var component = RenderComponent<Find>();

        component.FindAll("input#paymentrequestid")[0].Change("NotAPaymentRequestId");

        var button = component.FindAll("button.govuk-button")[0];
        button.Click();

        component.WaitForElements("ul.govuk-error-summary__list");

        component.Instance.Invoice.Should().BeNull();

        var errors = component.FindAll("ul.govuk-error-summary__list");

        errors.Should().NotBeNull();
        errors.Count.Should().Be(1);

        var errorItems = component.FindAll("ul.govuk-error-summary__list li");

        errorItems.Should().NotBeNull();
        errorItems.Count.Should().BeGreaterThan(0);
    }
}

