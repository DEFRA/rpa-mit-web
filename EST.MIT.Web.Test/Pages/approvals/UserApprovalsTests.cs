using Entities;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.approvals.UserApprovals;
using EST.MIT.Web.Shared;
using EST.MIT.Web.Shared.Components.ApprovalCard;
using Microsoft.Extensions.DependencyInjection;
using Services;

namespace Pages.Tests;

public class UserApprovalsTests : TestContext
{
    private readonly Mock<IApprovalService> _mockApprovalService;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    private Invoice _invoice;
    public UserApprovalsTests()
    {
        _invoice = new Invoice()
        {
            PaymentRequests = new List<PaymentRequest>() { new PaymentRequest() }
        };

        _mockApprovalService = new Mock<IApprovalService>();
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();

        Services.AddSingleton<IApprovalService>(_mockApprovalService.Object);
        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
    }

    [Fact]
    public void UserApproval_Renders()
    {
        var component = RenderComponent<UserApprovals>();
        component.Should().NotBeNull();
    }

    [Fact]
    public void UserApproval_Lists_ApprovalCards()
    {
        _mockApprovalService.Setup(x => x.GetOutstandingApprovalsAsync())
            .Returns(Task.FromResult(new List<Invoice>
                {_invoice}.AsEnumerable()
            ));

        var component = RenderComponent<UserApprovals>();

        component.FindComponents<ApprovalCard>().Count.Should().Be(1);

    }
}