using Entities;
using EST.MIT.Web.Shared;
using Microsoft.AspNetCore.Components;
using Services;

namespace EST.MIT.Web.Pages.approvals.UserApprovals;

public partial class UserApprovals : ComponentBase
{
    [Inject] private IApprovalService _approvalService { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }

    private IEnumerable<Invoice> _approvals;

    protected override async Task OnInitializedAsync()
    {
        _invoiceStateContainer.SetValue(null);
        _approvals = await _approvalService.GetOutstandingApprovalsAsync();
    }

}