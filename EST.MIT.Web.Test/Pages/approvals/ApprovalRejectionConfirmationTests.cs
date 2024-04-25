using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Pages.approvals.ApprovalRejectionConfirmation;

namespace EST.MIT.Web.Tests.Pages;

public class ApprovalRejectionConfirmationTests : TestContext
{
    private readonly Mock<IPageServices> _mockPageServices;
    private readonly Mock<IApprovalService> _mockApprovalService;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    private readonly Invoice _invoice;

    public ApprovalRejectionConfirmationTests()
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
    public void ApproveConfirmed_NavigatesToApprovedConfirmationPage_OnSuccess()
    {
        var navigationManager = new FakeNavigationManager();
        Services.AddSingleton<NavigationManager>(navigationManager);
        var component = RenderComponent<ApprovalRejectionConfirmation>();

        _mockApprovalService.Setup(x => x.ApproveInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(true);

        component.WaitForAssertion(() => navigationManager.Uri.Should().Contain("/approval/confirmation/approved"));

        navigationManager.Uri.Should().Contain("/approval/confirmation/approved");
    }

    private class FakeNavigationManager : NavigationManager, IDisposable
    {
        public string NavigateToLocation { get; private set; } = default!;
        private const string id = "3f915751-b2fa-49f0-853d-336b031833f8";
        public FakeNavigationManager()
        {
            Uri = $"http://localhost/approval/confirmation/approved/{id}";
            EnsureInitialized();
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            NavigateToLocation = uri;

            Uri = $"{this.BaseUri}{uri}";
        }

        protected sealed override void EnsureInitialized()
        {
            Initialize("http://localhost/", $"http://localhost/approval/confirmation/approved/{id}");
        }

        public void Dispose() { }
    }
}