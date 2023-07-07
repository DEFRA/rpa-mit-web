using System.Net;
using Entities;
using Helpers;
using Repositories;

namespace Services;

public interface IApprovalAPI
{
    Task<ApiResponse> GetApproversAsync(string scheme, string value);
    Task<ApiResponse<BoolRef>> ValidateApproverAsync(string approver);
}

public class ApprovalAPI : IApprovalAPI
{
    private readonly ILogger<ApprovalAPI> _logger;
    private readonly IApprovalRepository _approvalRepository;

    public ApprovalAPI(IApprovalRepository approvalRepository, ILogger<ApprovalAPI> logger)
    {
        _logger = logger;
        _approvalRepository = approvalRepository;
    }

    public async Task<ApiResponse> GetApproversAsync(string scheme, string value) => await GetApprovers(scheme, value);
    public async Task<ApiResponse<BoolRef>> ValidateApproverAsync(string approver) => await ValidateApprover(approver);

    private async Task<ApiResponse> GetApprovers(string scheme, string value)
    {
        var response = await _approvalRepository.GetApproversAsync(scheme, value);
        _logger.LogInformation($"ApprovalAPI.GetApproversAsync: response received from approval service: {response.StatusCode}");
        if (response.IsSuccessStatusCode)
        {
            return new ApiResponse(true, HttpStatusCode.OK)
            {
                Data = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>().ContinueWith(x =>
                {
                    if (x.Result.IsNull())
                    {
                        _logger.LogError("ApprovalAPI.GetApproversAsync: approvers dictionary is null");
                        return new Dictionary<string, string>();
                    }
                    return x.Result;
                })
            };
        }

        return new ApiResponse(false, response.StatusCode);
    }

    private async Task<ApiResponse<BoolRef>> ValidateApprover(string approver)
    {
        approver = approver.Trim().ToLower();
        var response = await _approvalRepository.ValidateApproverAsync(approver);
        _logger.LogInformation($"ApprovalAPI.ValidateApprover: response received from approval service: {response.StatusCode}");

        var reply = new ApiResponse<BoolRef>(response.StatusCode) { Data = new BoolRef(true) };

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (!bool.Parse(content))
            {
                _logger.LogError($"ApprovalAPI.ValidateApprover: {approver} is not a valid approver");
                reply.Data = new BoolRef(false);
                reply.Errors.Add("ApproverEmail", new List<string> { $"{approver} is not a valid approver" });
            }

            return reply;
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            _logger.LogWarning($"No content returned from API");
            reply.Data = new BoolRef(false);
            reply.Errors.Add($"{HttpStatusCode.NoContent}", new List<string> { $"No content returned from API" });
            return reply;
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError($"Invalid request was sent to API");
            reply.Data = default!;
            reply.Errors.Add($"{HttpStatusCode.BadRequest}", new List<string> { $"Invalid request was sent to API" });
            return reply;
        }

        _logger.LogError($"Unknown response from API");
        reply.Data = default!;
        reply.Errors.Add($"{response.StatusCode}", new List<string> { $"Unknown response from API" });
        return reply;

    }
}
