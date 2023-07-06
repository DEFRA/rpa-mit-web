using System.Net;
using System.Text;
using System.Text.Json;
using Entities;
using Microsoft.Extensions.Logging;
using Repositories;

namespace Services.Tests;

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


}