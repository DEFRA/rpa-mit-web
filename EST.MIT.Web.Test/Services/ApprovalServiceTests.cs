using System.Net;
using Entities;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Services.Tests;

public class ApprovalServiceTests
{
    private readonly Mock<IQueueService> _mockQueueService;
    private readonly Mock<IInvoiceAPI> _mockApiService;
    private readonly Mock<IApprovalService> _mockApprovalService;
    private readonly Mock<IApprovalAPI> _mockApprovalApi;

    public ApprovalServiceTests()
    {
        _mockQueueService = new Mock<IQueueService>();
        _mockApiService = new Mock<IInvoiceAPI>();
        _mockApprovalService = new Mock<IApprovalService>();
        _mockApprovalApi = new Mock<IApprovalAPI>();
    }

    [Fact]
    public void GetInvoiceAsync_ReturnsInvoice()
    {
        _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Invoice());

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());
        var response = service.GetInvoiceAsync("123", "BPS");

        response.Should().NotBeNull();
    }

    [Fact]
    public void ApproveInvoiceAsync_Returns_Success()
    {
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = service.ApproveInvoiceAsync(new Invoice() { SchemeType = "BPS" });
        response.Wait();

        response.Result.Should().BeTrue();
    }

    [Fact]
    public void RejectInvoiceAsync_Returns_Success()
    {
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = service.RejectInvoiceAsync(new Invoice() { SchemeType = "BPS" }, "Justification");

        response.Result.Should().BeTrue();
    }

    [Fact]
    public void Update_Invoice_Fails()
    {
        var apiResponse = new ApiResponse<Invoice>(HttpStatusCode.BadRequest)
        {
            Errors = new Dictionary<string, List<string>> { { "Error", new List<string> { "Error" } } }
        };

        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(apiResponse);
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        var mockLogger = new Mock<ILogger<ApprovalService>>();

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = service.ApproveInvoiceAsync(new Invoice() { SchemeType = "BPS" });
        response.Wait();

        response.Result.Should().BeFalse();
    }

    [Fact]
    public void Add_Message_To_Queue_Fails()
    {
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        var mockLogger = new Mock<ILogger<ApprovalService>>();

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = service.RejectInvoiceAsync(new Invoice() { SchemeType = "BPS" }, "Justification");

        response.Result.Should().BeFalse();
    }

    [Fact]
    public void UpdateAndNotify_Catches_Exception()
    {
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ThrowsAsync(new Exception());
        var mockLogger = new Mock<ILogger<ApprovalService>>();

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = service.ApproveInvoiceAsync(new Invoice() { SchemeType = "BPS" });

        response.Result.Should().BeFalse();
    }

    [Fact]
    public void SubmitApprovalAsync_Sends_Notification()
    {
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = service.SubmitApprovalAsync(new Invoice() { SchemeType = "BPS" });
        response.Wait();

        response.Result.Should().BeEquivalentTo(new ApiResponse<Invoice>(HttpStatusCode.OK));
    }

    [Fact]
    public void SubmitApprovalAsync_Value_Is_Correct()
    {
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

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

        var response = service.SubmitApprovalAsync(invoice);
        response.Wait();

        response.Result.Should().BeEquivalentTo(new ApiResponse<Invoice>(HttpStatusCode.OK));
    }

    [Fact]
    public void SubmitApprovalAsync_Fails_Notification()
    {
        _mockApiService.Setup(x => x.UpdateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(new ApiResponse<Invoice>(HttpStatusCode.OK));
        _mockQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        var mockLogger = new Mock<ILogger<ApprovalService>>();

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = service.SubmitApprovalAsync(new Invoice() { SchemeType = "BPS" });

        response.Result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void GetApproversAsync_Returns_Approvers()
    {
        _mockApprovalApi.Setup(x => x.GetApproversAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ApiResponse(true, HttpStatusCode.OK) { Data = new Dictionary<string, string> { { "name", "email" } } });

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = service.GetApproversAsync("BPS", "123");

        response.Result.Should().NotBeNull();
        response.Result.First().Key.Should().Be("name");
        response.Result.First().Value.Should().Be("email");
    }

    [Fact]
    public void GetApproversAsync_Returns_Empty_Approvers()
    {
        _mockApprovalApi.Setup(x => x.GetApproversAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ApiResponse(false, HttpStatusCode.OK) { Data = new Dictionary<string, string>() });

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = service.GetApproversAsync("BPS", "123");

        response.Result.Should().NotBeNull();
        response.Result.Count.Should().Be(0);
    }

    [Fact]
    public void GetApprovalAsync_Returns_Invoice()
    {
        var _invoice = new Invoice();
        _invoice.Update("approval");

        _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_invoice);

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = service.GetApprovalAsync("123", "BPS");

        response.Result.Should().NotBeNull();

    }

    [Fact]
    public void GetApprovalAsync_Invoice_Not_Found()
    {
        _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult<Invoice>(null));

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = service.GetApprovalAsync("123", "BPS");

        response.Result.Should().BeNull();

    }

    [Fact]
    public void GetApprovalAsync_Invoice_Not_Approval_Status()
    {
        var _invoice = new Invoice();
        _invoice.Update("notapproval");

        _mockApiService.Setup(x => x.FindInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_invoice);

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());

        var response = service.GetApprovalAsync("123", "BPS");

        response.Result.Should().BeNull();

    }

    [Fact]
    public void GetOutstandingApprovalsAsync_Returns_Invoices()
    {
        var _invoice = new Invoice();
        _invoice.Update("approval");

        _mockApiService.Setup(x => x.GetApprovalsAsync()).ReturnsAsync(new List<Invoice> { _invoice });

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());
        var response = service.GetOutstandingApprovalsAsync();

        response.Result.Should().NotBeNull();
        response.Result.Count().Should().Be(1);
    }

    [Fact]
    public void ValidateApproverAsync_Returns_True()
    {
        _mockApprovalApi.Setup(x => x.ValidateApproverAsync(It.IsAny<string>())).ReturnsAsync(new ApiResponse<BoolRef>(HttpStatusCode.OK) { Data = new BoolRef(true) });

        var service = new ApprovalService(_mockQueueService.Object, _mockApiService.Object, _mockApprovalApi.Object, Mock.Of<ILogger<ApprovalService>>(), Mock.Of<IHttpContextAccessor>());
        var response = service.ValidateApproverAsync("Loid.Forger@defra.gov.uk");

        response.Result.Data.Value.Should().BeTrue();
    }

}