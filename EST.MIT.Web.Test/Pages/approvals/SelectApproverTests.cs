using System.Net;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.approvals.SelectApprover;
using EST.MIT.Web.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Shared;

namespace EST.MIT.Web.Tests.Pages;

public class SelectApproverTests : TestContext
{
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    private readonly Mock<IPageServices> _mockPageServices;
    private readonly Mock<IApprovalService> _mockApprovalService;

    public SelectApproverTests()
    {
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();
        _mockPageServices = new Mock<IPageServices>();
        _mockApprovalService = new Mock<IApprovalService>();

        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
        Services.AddSingleton<IPageServices>(_mockPageServices.Object);
        Services.AddSingleton<IApprovalService>(_mockApprovalService.Object);
    }

    [Fact]
    public void AfterRender_Redirects_When_Null_Invoice()
    {
        var component = RenderComponent<SelectApprover>(parameters => parameters.Add(p => p.Layout, new MainLayout()));

        var navigationManager = Services.GetService<NavigationManager>();
        component.WaitForAssertion(() => navigationManager?.Uri.Should().Be("http://localhost/"));
    }

    [Fact]
    public void No_Input_Fails_Validation()
    {
        _mockPageServices.Setup(x => x.Validation(It.IsAny<ApproverSelect>(), out It.Ref<bool>.IsAny, out It.Ref<Dictionary<string, List<string>>>.IsAny))
            .Callback((object approverSelect, out bool IsErrored, out Dictionary<string, List<string>> errors) =>
            {
                IsErrored = true;
                errors = new()
                {
                    { "approveremail", new List<string>() { "Please enter an email address" } }
                };

            });

        _mockInvoiceStateContainer.SetupGet(x => x.Value).Returns(new Invoice());

        var component = RenderComponent<SelectApprover>(parameters => parameters.Add(p => p.Layout, new MainLayout()));
        component.FindAll("button[type=submit]")[0].Click();

        component.WaitForElements("p.govuk-error-message");

        var errorMessages = component.FindAll("p.govuk-error-message");

        errorMessages.Should().NotBeEmpty();
        errorMessages.Should().HaveCount(1);

    }

    [Fact]
    public void ConfirmDelete_Navigates_To_Summary_On_Cancel()
    {
        var _invoice = new Invoice() { SchemeType = "BPS" };
        _mockInvoiceStateContainer.Setup(x => x.Value).Returns(_invoice);

        var component = RenderComponent<SelectApprover>(parameters => parameters.Add(p => p.Layout, new MainLayout()));

        component.FindAll("a.govuk-link")[0].Click();

        var navigationManager = Services.GetService<NavigationManager>();
        component.WaitForAssertion(() => navigationManager.Uri.Should().Be($"http://localhost/invoice/summary/{_invoice.SchemeType}/{_invoice.Id}"));
    }

    [Fact]
    public void SubmitApproval_Success()
    {
        var navigationManager = Services.GetService<NavigationManager>();
        var testInvoice = new Invoice();

        _mockInvoiceStateContainer.Setup(x => x.Value).Returns(testInvoice);
        _mockApprovalService.Setup(x => x.ValidateApproverAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ApiResponse<BoolRef>(HttpStatusCode.OK) { Data = new BoolRef(true) });
        _mockApprovalService.Setup(x => x.SubmitApprovalAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));

        var component = RenderComponent<SelectApprover>(parameters => parameters.Add(p => p.Layout, new MainLayout()));
        component.FindAll("input[type=text]")[0].Change("Loid.Forger@defra.gov.uk");
        component.FindAll("button[type=submit]")[0].Click();

        component.WaitForAssertion(() => navigationManager.Uri.Should().Be($"http://localhost/approval/confirmation/{testInvoice.Id}"));
    }

    [Fact]
    public void SubmitApproval_Validate_Fails()
    {
        _mockInvoiceStateContainer.Setup(x => x.Value).Returns(new Invoice());
        _mockApprovalService.Setup(x => x.ValidateApproverAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ApiResponse<BoolRef>(HttpStatusCode.OK) { Data = new BoolRef(false), Errors = new Dictionary<string, List<string>>() { { "test", new List<string> { "test" } } } });
        _mockApprovalService.Setup(x => x.SubmitApprovalAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));

        var component = RenderComponent<SelectApprover>(parameters => parameters.Add(p => p.Layout, new MainLayout()));
        component.FindAll("input[type=text]")[0].Change("Loid.Forger@defra.gov.uk");
        component.FindAll("button[type=submit]")[0].Click();

        component.WaitForElements("ul.govuk-error-summary__list");

        var errorMessages = component.FindAll("ul.govuk-error-summary__list > li");

        errorMessages.Count.Should().Be(1);

        _mockApprovalService.Setup(x => x.ValidateApproverAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ApiResponse<BoolRef>(HttpStatusCode.BadRequest) { Data = new BoolRef(true), Errors = new Dictionary<string, List<string>>() { { "test", new List<string> { "test" } } } });
        component = RenderComponent<SelectApprover>(parameters => parameters.Add(p => p.Layout, new MainLayout()));
        component.FindAll("input[type=text]")[0].Change("Loid.Forger@defra.gov.uk");
        component.FindAll("button[type=submit]")[0].Click();

        component.WaitForElements("ul.govuk-error-summary__list");

        errorMessages = component.FindAll("ul.govuk-error-summary__list > li");

        errorMessages.Count.Should().Be(1);

    }

    [Fact]
    public void SubmitApproval_Submit_Fails()
    {
        Invoice invoice = new Invoice();
        Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>() { { "test", new List<string> { "test" } } };
        invoice.AddErrors(errors);
        _mockInvoiceStateContainer.Setup(x => x.Value).Returns(invoice);
        _mockApprovalService.Setup(x => x.ValidateApproverAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ApiResponse<BoolRef>(HttpStatusCode.OK) { Data = new BoolRef(true) });
        _mockApprovalService.Setup(x => x.SubmitApprovalAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.BadRequest) { Errors = errors });

        var component = RenderComponent<SelectApprover>(parameters => parameters.Add(p => p.Layout, new MainLayout()));
        component.FindAll("input[type=text]")[0].Change("Loid.Forger@defra.gov.uk");
        component.FindAll("button[type=submit]")[0].Click();

        component.FindAll("div.govuk-error-summary").Count.Should().Be(1);

    }
}