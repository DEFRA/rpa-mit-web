using System.Net;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Services;
using EST.MIT.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Test.Services;

public class ApprovalServiceTests
{
    private readonly Mock<IEventQueueService> _mockQueueService;
    private readonly Mock<INotificationQueueService> _mockNotificationQueueService;
    private readonly Mock<IInvoiceAPI> _mockApiService;
    private readonly Mock<IApprovalService> _mockApprovalService;
    private readonly Mock<IApprovalAPI> _mockApprovalApi;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

    public ApprovalServiceTests()
    {
        _mockQueueService = new Mock<IEventQueueService>();
        _mockNotificationQueueService = new Mock<INotificationQueueService>();
        _mockApiService = new Mock<IInvoiceAPI>();
        _mockApprovalService = new Mock<IApprovalService>();
        _mockApprovalApi = new Mock<IApprovalAPI>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
    }

    [Fact]
    public void GetInvoiceAsync_ReturnsInvoice()
    {
        _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Invoice());

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = service.GetInvoiceAsync("123", "BPS");

        response.Should().NotBeNull();
    }

    [Fact]
    public async void ApproveInvoiceAsync_Returns_Success()
    {
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _mockNotificationQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<Notification>())).ReturnsAsync(true);

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = await service.ApproveInvoiceAsync(new Invoice() { SchemeType = "BPS" });

        response.Should().BeTrue();
    }

    [Fact]
    public async void RejectInvoiceAsync_Returns_Success()
    {
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _mockNotificationQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<Notification>())).ReturnsAsync(true);

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = await service.RejectInvoiceAsync(new Invoice() { SchemeType = "BPS" }, "Reason");

        response.Should().BeTrue();
    }

    [Fact]
    public async void Update_Invoice_Fails()
    {
        var apiResponse = new ApiResponse<Invoice>(HttpStatusCode.BadRequest)
        {
            Errors = new Dictionary<string, List<string>> { { "Error", new List<string> { "Error" } } }
        };

        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(apiResponse);
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        var mockLogger = new Mock<ILogger<ApprovalService>>();

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = await service.ApproveInvoiceAsync(new Invoice() { SchemeType = "BPS" });

        response.Should().BeFalse();
    }

    [Fact]
    public async void Add_Message_To_Queue_Fails()
    {
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        var mockLogger = new Mock<ILogger<ApprovalService>>();

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = await service.RejectInvoiceAsync(new Invoice() { SchemeType = "BPS" }, "Justification");

        response.Should().BeFalse();
    }

    [Fact]
    public async void UpdateAndNotify_Catches_Exception()
    {
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ThrowsAsync(new Exception());
        var mockLogger = new Mock<ILogger<ApprovalService>>();

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = await service.ApproveInvoiceAsync(new Invoice() { SchemeType = "BPS" });

        response.Should().BeFalse();
    }

    [Fact]
    public async void SubmitApprovalAsync_Sends_Notification()
    {
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _mockNotificationQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<Notification>())).ReturnsAsync(true);

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = await service.SubmitApprovalAsync(new Invoice() { SchemeType = "BPS" });

        response.Should().BeEquivalentTo(new ApiResponse<Invoice>(HttpStatusCode.OK));
    }

    [Fact]
    public async void SubmitApprovalAsync_Value_Is_Correct()
    {
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _mockNotificationQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<Notification>())).ReturnsAsync(true);

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var invoice = new Invoice()
        {
            SchemeType = "BPS",
            PaymentRequests = new List<PaymentRequest>()
            {
                new PaymentRequest()
                {
                    Value = 100
                },
                new PaymentRequest()
                {
                    Value = 200
                }
            }
        };

        var response = await service.SubmitApprovalAsync(invoice);

        response.Should().BeEquivalentTo(new ApiResponse<Invoice>(HttpStatusCode.OK));
    }

    [Fact]
    public async void SubmitApprovalAsync_Fails_Notification()
    {
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        _mockNotificationQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<Notification>())).ReturnsAsync(false);

        var mockLogger = new Mock<ILogger<ApprovalService>>();

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = await service.SubmitApprovalAsync(new Invoice() { SchemeType = "BPS" });

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void GetApproversAsync_Returns_Approvers()
    {
        _mockApprovalApi.Setup(x => x.GetApproversAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ApiResponse(true, HttpStatusCode.OK) { Data = new Dictionary<string, string> { { "name", "email" } } });

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = await service.GetApproversAsync("BPS", "123");

        response.Should().NotBeNull();
        response.First().Key.Should().Be("name");
        response.First().Value.Should().Be("email");
    }

    [Fact]
    public async void GetApproversAsync_Returns_Empty_Approvers()
    {
        _mockApprovalApi.Setup(x => x.GetApproversAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ApiResponse(false, HttpStatusCode.OK) { Data = new Dictionary<string, string>() });

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = await service.GetApproversAsync("BPS", "123");

        response.Should().NotBeNull();
        response.Count.Should().Be(0);
    }

    //[Fact]
    //public void GetApprovalAsync_Returns_Invoice()
    //{
    //    var _invoice = new Invoice();
    //    _invoice.Update(InvoiceStatuses.AwaitingApproval);

    //    _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_invoice);

    //    var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

    //    var response = service.GetApprovalAsync("123", "BPS");

    //    response.Result.Should().NotBeNull();

    //}

    //[Fact]
    //public void GetApprovalAsync_Invoice_Not_Found()
    //{
    //    _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
    //        .Returns(Task.FromResult<Invoice>(null));

    //    var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

    //    var response = service.GetApprovalAsync("123", "BPS");

    //    response.Result.Should().BeNull();

    ////}

    //[Fact]
    //public void GetApprovalAsync_Invoice_Not_Approval_Status()
    //{
    //    var _invoice = new Invoice();
    //    _invoice.Update("notapproval");

    //    _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
    //        .ReturnsAsync(_invoice);

    //    var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

    //    var response = service.GetApprovalAsync("123", "BPS");

    //    response.Result.Should().BeNull();

    //}

    //[Fact]
    //public void GetOutstandingApprovalsAsync_Returns_Invoices()
    //{
    //    var _invoice = new Invoice();
    //    _invoice.Update(InvoiceStatuses.AwaitingApproval);

    //    _mockApiService.Setup(x => x.GetAllApprovalInvoicesAsync()).ReturnsAsync(new List<Invoice> { _invoice });

    //    var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());
    //    var response = service.GetOutstandingApprovalsAsync();

    //    response.Result.Should().NotBeNull();
    //    response.Result.Count().Should().Be(1);
    //}

    [Fact]
    public async void ValidateApproverAsync_Returns_True()
    {
        _mockApprovalApi.Setup(x => x.ValidateApproverAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ApiResponse<BoolRef>(HttpStatusCode.OK) { Data = new BoolRef(true) });

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());
        var response = await service.ValidateApproverAsync("Loid.Forger@defra.gov.uk", "schemeType");

        response.Data.Value.Should().BeTrue();
    }

    [Fact]
    public async Task ApproveInvoiceAsync_Returns_Failure_When_EventQueue_Fails()
    {
        var testInvoice = new Invoice { SchemeType = "TestScheme" };
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object,
            _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(),
            Mock.Of<IHttpContextAccessor>());

        var response = await service.ApproveInvoiceAsync(testInvoice);

        response.Should().BeFalse();
    }
    [Fact]
    public async Task RejectInvoiceAsync_Returns_Failure_When_NotificationQueue_Fails()
    {
        var testInvoice = new Invoice { SchemeType = "TestScheme" };
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockNotificationQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<Notification>()))
            .ReturnsAsync(false);

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object,
            _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(),
            Mock.Of<IHttpContextAccessor>());

        var response = await service.RejectInvoiceAsync(testInvoice, "Justification");

        response.Should().BeFalse();
    }

    [Fact]
    public async Task SubmitApprovalAsync_Returns_Failure_On_Exception()
    {
        var testInvoice = new Invoice { SchemeType = "TestScheme" };
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>()))
            .ThrowsAsync(new Exception("Test exception"));

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object,
            _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(),
            Mock.Of<IHttpContextAccessor>());

        var response = await service.SubmitApprovalAsync(testInvoice);

        response.IsSuccess.Should().BeFalse();
        response.Errors.Should().ContainKey("Exception");
        response.Errors["Exception"].Should().Contain("Test exception");
    }

    [Fact]
    public async Task GetApproversAsync_Returns_Empty_When_Api_Fails()
    {
        _mockApprovalApi.Setup(x => x.GetApproversAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponse(false, HttpStatusCode.BadRequest));

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object,
            _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(),
            Mock.Of<IHttpContextAccessor>());

        var approvers = await service.GetApproversAsync("scheme", "value");

        approvers.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateApproverAsync_Returns_False_When_ApiResponse_IsNotFound()
    {
        _mockApprovalApi.Setup(x => x.ValidateApproverAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponse<BoolRef>(HttpStatusCode.NotFound) { Data = new BoolRef(false) });

        var service = new ApprovalService(_mockQueueService.Object, _mockNotificationQueueService.Object,
            _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(),
            Mock.Of<IHttpContextAccessor>());

        var result = await service.ValidateApproverAsync("approver@example.com", "scheme");

        result.Data.Value.Should().BeFalse();
    }
}