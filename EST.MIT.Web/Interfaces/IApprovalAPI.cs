using EST.MIT.Web.Entities;
using EST.MIT.Web.Helpers;

namespace EST.MIT.Web.Interfaces;

public interface IApprovalAPI
{
    Task<ApiResponse> GetApproversAsync(string scheme, string value);
    Task<ApiResponse<BoolRef>> ValidateApproverAsync(string approver);
}
