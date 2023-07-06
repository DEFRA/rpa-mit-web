using Entities;
using EST.MIT.Web.Pages.approvals.SelectApprover;
using EST.MIT.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Services;

namespace Pages.Tests;

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
    public void SelectApprover_Is_Not_Errored()
    {
        var component = RenderComponent<SelectApprover>();
        var errors = component.FindAll("p.govuk-error-message");

        errors.Should().BeEmpty();
    }

    // [Fact]
    // public void SelectApprover_Is_Errored()
    // {
    //     _mockPageServices.Setup(x => x.Validation(It.IsAny<Approver>(), out It.Ref<bool>.IsAny, out It.Ref<Dictionary<string, string>>.IsAny))
    //         .Callback((object approver, out bool IsErrored, out Dictionary<string, string> errors) =>
    //         {
    //             IsErrored = true;
    //             errors = new();
    //             errors.Add("Name", "Name is required");
    //         });

    //     var component = RenderComponent<SelectApprover>();
    //     component.FindAll("button[type=submit]")[0].Click();

    //     component.FindAll("p.govuk-error-message").Count.Should().Be(1);

    // }

    [Fact]
    public void SubmitApproval()
    {
        var navigationManager = Services.GetService<NavigationManager>();
        var testInvoice = new Invoice();

        _mockInvoiceStateContainer.Setup(x => x.Value).Returns(testInvoice);
        _mockApprovalService.Setup(x => x.SubmitApprovalAsync(It.IsAny<Invoice>())).ReturnsAsync(true);

        var component = RenderComponent<SelectApprover>();
        component.FindAll("input[type=radio]")[0].Change("1");
        component.FindAll("button[type=submit]")[0].Click();

        navigationManager.Uri.Should().Be($"http://localhost/approval/confirmation/{testInvoice.Id}");
    }
}