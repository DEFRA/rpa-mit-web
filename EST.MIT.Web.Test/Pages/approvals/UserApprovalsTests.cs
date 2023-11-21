using EST.MIT.Web.Entities;
using EST.MIT.Web.Pages.approvals.UserApprovals;
using Microsoft.Extensions.DependencyInjection;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Tests.Pages;

public class UserApprovalsTests : TestContext
{
    private readonly Mock<IApprovalService> _mockApprovalService;
    private readonly Mock<IInvoiceStateContainer> _mockInvoiceStateContainer;
    private readonly Mock<IInvoiceAPI> _mockApiService;
    private readonly Invoice _invoice;
    public UserApprovalsTests()
    {
        _invoice = new Invoice()
        {
            PaymentRequests = new List<PaymentRequest>() { new PaymentRequest() }
        };

        _mockApprovalService = new Mock<IApprovalService>();
        _mockInvoiceStateContainer = new Mock<IInvoiceStateContainer>();
        _mockApiService = new Mock<IInvoiceAPI>();

        Services.AddSingleton<IApprovalService>(_mockApprovalService.Object);
        Services.AddSingleton<IInvoiceStateContainer>(_mockInvoiceStateContainer.Object);
        Services.AddSingleton<IInvoiceAPI>(_mockApiService.Object);
    }

    [Fact]
    public void UserApproval_Renders()
    {
        _mockApiService.Setup(x => x.GetAllApprovalInvoicesAsync()).ReturnsAsync(new List<Invoice>());
        var component = RenderComponent<UserApprovals>();
        component.Should().NotBeNull();
    }

    //[Fact]
    //public void UserApproval_Lists_ApprovalCards()
    //{
    //    _mockApprovalService.Setup(x => x.GetOutstandingApprovalsAsync())
    //        .Returns(Task.FromResult(new List<Invoice>
    //            {_invoice}.AsEnumerable()
    //        ));

    //    var component = RenderComponent<UserApprovals>();

    //    component.FindComponents<ApprovalCard>().Count.Should().Be(1);

    //}
}