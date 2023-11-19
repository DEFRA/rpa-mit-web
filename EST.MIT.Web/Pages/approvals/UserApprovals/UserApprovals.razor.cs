using EST.MIT.Web.Entities;
using EST.MIT.Web.Shared;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Models;

namespace EST.MIT.Web.Pages.approvals.UserApprovals;

public partial class UserApprovals : ComponentBase
{
    [Inject] private IInvoiceAPI InvoiceApi { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }

    private IEnumerable<Invoice> _approvals;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _invoiceStateContainer.SetValue(null);

        _approvals = await InvoiceApi.GetAllApprovalInvoicesAsync();
	}
}