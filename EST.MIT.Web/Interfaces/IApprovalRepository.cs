namespace EST.MIT.Web.Interfaces;

public interface IApprovalRepository
{
    Task<HttpResponseMessage> GetApproversAsync(string scheme, string value);
    Task<HttpResponseMessage> ValidateApproverAsync(string approver, string approvalGroup);
}
