using EST.MIT.Web.Entities;
using EST.MIT.Web.Helpers;

namespace EST.MIT.Web.Interfaces;

public interface IApprovalService
{
    Task<Invoice> GetInvoiceAsync(string id, string scheme);
    Task<bool> ApproveInvoiceAsync(Invoice invoice);
    Task<bool> RejectInvoiceAsync(Invoice invoice, string reason);
    Task<ApiResponse<Invoice>> SubmitApprovalAsync(Invoice invoice);
    Task<Dictionary<string, string>> GetApproversAsync(string scheme, string value);
    Task<ApiResponse<BoolRef>> ValidateApproverAsync(string approver, string scheme);
}
