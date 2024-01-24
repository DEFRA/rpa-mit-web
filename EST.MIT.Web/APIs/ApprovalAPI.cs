using System.Net;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Helpers;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EST.MIT.Web.APIs;

public class ApprovalAPI : IApprovalAPI
{
    private readonly ILogger<ApprovalAPI> _logger;
    private readonly IApprovalRepository _approvalRepository;

    public ApprovalAPI(IApprovalRepository approvalRepository, ILogger<ApprovalAPI> logger)
    {
        _logger = logger; 
        _approvalRepository = approvalRepository;
    }
    public async Task<ApiResponse> GetApproversAsync(string scheme, string value)
    {
        var response = await _approvalRepository.GetApproversAsync(scheme, value);
        _logger.LogInformation($"ApprovalAPI.GetApproversAsync: response received from approval service: {response.StatusCode}");

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var approversDictionary = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (approversDictionary == null || approversDictionary.Count == 0)
                {
                    _logger.LogError("ApprovalAPI.GetApproversAsync: approvers dictionary is null");
                    approversDictionary = new Dictionary<string, string>();
                }

                return new ApiResponse(true, HttpStatusCode.OK) { Data = approversDictionary };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApprovalAPI.GetApproversAsync: Error reading JSON content");
                // Handle the exception as per your error handling policy
                // For example, you might want to return a different ApiResponse here
            }
        }

        return new ApiResponse(false, response.StatusCode);
    }


    public async Task<ApiResponse<BoolRef>> ValidateApproverAsync(string approver, string approvalGroup)
    {
        approver = approver.Trim().ToLower();
        var response = await _approvalRepository.ValidateApproverAsync(approver, approvalGroup);
        _logger.LogInformation($"ApprovalAPI.ValidateApprover: response received from approval service: {response.StatusCode}");

        var reply = new ApiResponse<BoolRef>(response.StatusCode) { Data = new BoolRef(true) };

        if (response.StatusCode == HttpStatusCode.OK)
        {
            _logger.LogError($"ApprovalAPI.ValidateApprover: {approver} is a valid approver");
            reply.Data = new BoolRef(true);
            return reply;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogError($"ApprovalAPI.ValidateApprover: {approver} is not a valid approver");
            reply.Data = new BoolRef(false);
            reply.Errors.Add("ApproverEmail", new List<string> { $"{approver} is not a valid approver for the {approvalGroup} group" });
            return reply;
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var errors = new List<string>();
            var message = await response.Content.ReadAsStringAsync();
            var validationErrors = Newtonsoft.Json.JsonConvert.DeserializeObject<ValidationProblemDetails>(message);
            _logger.LogError($"Invalid request was sent to API");

            reply.Data = default!;

            if (validationErrors != null)
            {
                foreach (var error in validationErrors.Errors)
                {
                    foreach (var errorValue in error.Value)
                    {
                        errors.Add(errorValue);
                    }
                }
                reply.Errors.Add($"{HttpStatusCode.BadRequest}", errors);
            } else
            {
                reply.Errors.Add($"{HttpStatusCode.BadRequest}", new List<string> { $"Invalid request was sent to Approval API" });
            }
            return reply;
        }

        _logger.LogError($"Unknown response from API");
        reply.Data = default!;
        reply.Errors.Add($"{response.StatusCode}", new List<string> { $"Unknown response from API" });
        return reply;
    }
}