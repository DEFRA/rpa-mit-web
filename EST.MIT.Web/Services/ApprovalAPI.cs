using System.Net;
using Entities;
using Helpers;
using Repositories;

namespace Services;

public interface IApprovalAPI
{
    Task<ApiResponse> GetApproversAsync(string scheme, string value);
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

    public async Task<ApiResponse> GetApproversAsync(string scheme, string value) => await GetApproversImplementationAsync(scheme, value);

    private async Task<ApiResponse> GetApproversImplementationAsync(string scheme, string value)
    {
        var response = await _approvalRepository.GetApproversAsync(scheme, value);
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
}


