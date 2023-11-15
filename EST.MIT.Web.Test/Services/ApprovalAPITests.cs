using System.Net;
using System.Text;
using System.Text.Json;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Services;
using EST.MIT.Web.Helpers;
using Microsoft.Extensions.Logging;
using EST.MIT.Web.Repositories;

namespace EST.MIT.Web.Test.Services;

public class ApprovalAPITests
{
    private readonly Mock<IApprovalRepository> _mockApprovalRepository;
    private readonly Mock<ILogger<ApprovalAPI>> _logger;
    private readonly ApprovalAPI _approvalAPI;

    public ApprovalAPITests()
    {
        _mockApprovalRepository = new Mock<IApprovalRepository>();
        _logger = new Mock<ILogger<ApprovalAPI>>();
        _approvalAPI = new ApprovalAPI(_mockApprovalRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task GetApproversAsync_ReturnsApiResponse_WithApproversDictionary()
    {
        var expectedApprovers = new Dictionary<string, string>
        {
            { "testKey1", "testValue1" },
            { "testKey2", "testValue2" }
        };
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(expectedApprovers), Encoding.UTF8, "application/json")
        };

        _mockApprovalRepository.Setup(x => x.GetApproversAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(response);

        var result = await _approvalAPI.GetApproversAsync("testScheme", "testValue");

        result.Should().BeEquivalentTo(new ApiResponse(true, HttpStatusCode.OK)
        {
            Data = expectedApprovers
        });
    }

    [Fact]
    public async Task GetApproversAsync_ReturnsApiResponse_WithFailureStatusCode()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        _mockApprovalRepository.Setup(x => x.GetApproversAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(response);

        var result = await _approvalAPI.GetApproversAsync("testScheme", "testValue");

        result.Should().BeEquivalentTo(new ApiResponse(false, HttpStatusCode.BadRequest));
    }

    [Fact]
    public async Task GetApproversAsync_ReturnsApiResponse_WithEmptyApproversDictionary_WhenResponseContentIsNull()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        };
        _mockApprovalRepository.Setup(x => x.GetApproversAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(response);

        var result = await _approvalAPI.GetApproversAsync("testScheme", "testValue");

        result.Should().BeEquivalentTo(new ApiResponse(true, HttpStatusCode.OK)
        {
            Data = new Dictionary<string, string>()
        });
    }

    [Fact]
    public async Task ValidateApprover_ReturnsApiResponseOK_WithBoolRefTrue()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("true")
        };
        _mockApprovalRepository.Setup(x => x.ValidateApproverAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(response);

        var result = await _approvalAPI.ValidateApproverAsync("testApprover","schemeType");

        result.Should().BeEquivalentTo(new ApiResponse<BoolRef>(HttpStatusCode.OK)
        {
            Data = new BoolRef(true)
        });
    }

    [Fact]
    public async Task ValidateApprover_ReturnsApiResponseNotFound_WithBoolRefFalse()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("false")
        };
        _mockApprovalRepository.Setup(x => x.ValidateApproverAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(response);

        var result = await _approvalAPI.ValidateApproverAsync("testApprover", "schemeType");

        result.Should().BeEquivalentTo(new ApiResponse<BoolRef>(HttpStatusCode.NotFound)
        {
            Data = new BoolRef(false),
            Errors = new Dictionary<string, List<string>>
            {
                {$"ApproverEmail", new List<string> { "testapprover is not a valid approver" } }
            }
        });
    }

    [Fact]
    public async Task ValidateApprover_ReturnsApiResponseBadRequest_WithBoolRefNull()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

        _mockApprovalRepository.Setup(x => x.ValidateApproverAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(response);

        var result = await _approvalAPI.ValidateApproverAsync("testApprover", "schameType");

        result.Should().BeEquivalentTo(new ApiResponse<BoolRef>(HttpStatusCode.BadRequest)
        {
            Errors = new Dictionary<string, List<string>>()
            {
                {$"{HttpStatusCode.BadRequest}", new List<string> { "Invalid request was sent to API" }}
            }
        });
    }

    [Fact]
    public async Task ValidateApprover_ReturnsApiResponseUnknown_WithBoolRefNull()
    {
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        _mockApprovalRepository.Setup(x => x.ValidateApproverAsync(It.IsAny<string>(),It.IsAny<string>())).ReturnsAsync(response);

        var result = await _approvalAPI.ValidateApproverAsync("testApprover","schameType");

        result.Should().BeEquivalentTo(new ApiResponse<BoolRef>(HttpStatusCode.InternalServerError)
        {
            Errors = new Dictionary<string, List<string>>()
            {
                {$"{response.StatusCode}", new List<string> { "Unknown response from API" }}
            }
        });
    }
}