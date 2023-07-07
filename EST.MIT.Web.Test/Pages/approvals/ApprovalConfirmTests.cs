using EST.MIT.Web.Pages.approvals.ApprovalConfirm;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Entities;
using Services;
using EST.MIT.Web.Shared;

namespace Pages.Tests;

public class ApprovalConfirmTests : TestContext
{
    private readonly Mock<IPageServices> _mockPageServices;
    private readonly Mock<IApprovalService> _mockApprovalService;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    private readonly Invoice _invoice;

    public ApprovalConfirmTests()
    {
        _invoice = new Invoice
        {
            AccountType = "Account Type",
            SchemeType = "Scheme Type",
            PaymentType = "Invoice Type",
            Organisation = "Organisation"
        };

        _mockPageServices = new Mock<IPageServices>();
        _mockApprovalService = new Mock<IApprovalService>();
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();

        _mockInvoiceStateContainer.Setup(x => x.Value).Returns(_invoice);

        Services.AddSingleton<IPageServices>(_mockPageServices.Object);
        Services.AddSingleton<IApprovalService>(_mockApprovalService.Object);
        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
    }

    [Fact]
    public void Approve_Route_Renders_Successfully()
    {
        var component = RenderComponent<ApprovalConfirm>();
        var title = component.FindAll("h2.govuk-heading-m")[0];

        title.InnerHtml.Should().Be("Are you sure you want to approve this Invoice?");
    }

    [Fact]
    public void ApproveConfirmed_Calls_ApproveInvoiceAsync()
    {
        _mockApprovalService.Setup(x => x.ApproveInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(true);

        var component = RenderComponent<ApprovalConfirm>();
        component.FindAll("button")[0].Click();

        _mockApprovalService.Verify(x => x.ApproveInvoiceAsync(It.IsAny<Invoice>()), Times.Once);
    }

    [Fact]
    public void ApproveConfirmed_Navigates_To_Confirmation()
    {
        var navigationManager = Services.GetService<NavigationManager>();
        _mockApprovalService.Setup(x => x.ApproveInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(true);

        var component = RenderComponent<ApprovalConfirm>();

        component.FindAll("button")[0].Click();
        component.WaitForAssertion(() => navigationManager.Uri.Should().Contain("/approval/confirmation/"));

        navigationManager.Uri.Should().Contain("/approval/confirmation/");

    }

    [Fact]
    public void ApproveConfirmed_Handles_Errors()
    {
        _mockApprovalService.Setup(x => x.ApproveInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(false);

        var component = RenderComponent<ApprovalConfirm>();

        component.FindAll("button")[0].Click();
        component.WaitForElements("ul.govuk-error-summary__list");

        var errors = component.FindAll("ul.govuk-error-summary__list");

        errors.Should().NotBeNull();
        errors.Count.Should().Be(1);
    }


    [Fact]
    public void Reject_Route_Renders_Successfully()
    {
        var nav = new FakeNavigationManager();
        Services.AddSingleton<NavigationManager>(nav);

        var component = RenderComponent<ApprovalConfirm>();

        var title = component.FindAll("h2.govuk-heading-m")[0];
        var justificationElement = component.FindAll("textarea")[0];

        title.InnerHtml.Should().Be("Are you sure you want to reject this Invoice?");
        justificationElement.Should().NotBeNull();
    }

    [Fact]
    public void RejectConfirmed_Calls_RejectInvoiceAsync()
    {
        _mockApprovalService.Setup(x => x.RejectInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<string>())).ReturnsAsync(true);

        var nav = new FakeNavigationManager();
        Services.AddSingleton<NavigationManager>(nav);

        var component = RenderComponent<ApprovalConfirm>();
        component.FindAll("textarea")[0].Change("Test");
        component.FindAll("button")[0].Click();

        _mockApprovalService.Verify(x => x.RejectInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void RejectConfirmed_Navigates_To_Confirmation()
    {
        _mockApprovalService.Setup(x => x.RejectInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<string>())).ReturnsAsync(true);

        var nav = new FakeNavigationManager();
        Services.AddSingleton<NavigationManager>(nav);

        var component = RenderComponent<ApprovalConfirm>();
        component.FindAll("textarea")[0].Change("Test");
        component.FindAll("button")[0].Click();

        component.WaitForAssertion(() => nav.Uri.Should().Contain("/approval/confirmation/"));

        nav.Uri.Should().Contain("/approval/confirmation/");
    }

    [Fact]
    public void RejectConfirmed_Handles_Errors()
    {
        _mockApprovalService.Setup(x => x.RejectInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<string>())).ReturnsAsync(false);

        var nav = new FakeNavigationManager();
        Services.AddSingleton<NavigationManager>(nav);

        var component = RenderComponent<ApprovalConfirm>();

        component.FindAll("textarea")[0].Change("Test");
        component.FindAll("button")[0].Click();
        component.WaitForElements("ul.govuk-error-summary__list");

        var errors = component.FindAll("div.govuk-error-summary");

        errors.Should().NotBeNull();
        errors.Count.Should().Be(1);
    }

    [Fact]
    public void RejectConfirmed_Fails_Validation()
    {
        _mockApprovalService.Setup(x => x.RejectInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<string>())).ReturnsAsync(true);
        _mockPageServices.Setup(x => x.Validation(It.IsAny<ApproverSelect>(), out It.Ref<bool>.IsAny, out It.Ref<Dictionary<string, List<string>>>.IsAny))
            .Returns(false);

        var nav = new FakeNavigationManager();
        Services.AddSingleton<NavigationManager>(nav);

        var component = RenderComponent<ApprovalConfirm>();
        component.FindAll("button")[0].Click();

        _mockPageServices.Invocations.Count.Should().Be(1);
    }

    private class FakeNavigationManager : NavigationManager, IDisposable
    {
        public string NavigateToLocation { get; private set; } = default!;
        public FakeNavigationManager()
        {
            Uri = "http://localhost/approval/confirm/reject";
            EnsureInitialized();
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            NavigateToLocation = uri;

            Uri = $"{this.BaseUri}{uri}";
        }

        protected sealed override void EnsureInitialized()
        {
            Initialize("http://localhost/", "http://localhost/approval/confirm/reject");
        }

        public void Dispose() { }
    }

}