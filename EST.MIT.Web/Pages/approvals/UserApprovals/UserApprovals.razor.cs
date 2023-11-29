using EST.MIT.Web.Entities;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.approvals.UserApprovals;

public partial class UserApprovals : ComponentBase
{
    [Inject] private IInvoiceAPI InvoiceApi { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private ILogger<UserApprovals> Logger { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    private IEnumerable<Invoice> _approvals;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();

            _invoiceStateContainer.SetValue(null);

            _approvals = await InvoiceApi.GetAllApprovalInvoicesAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing UserApprovals page");
            NavigationManager.NavigateTo("/error");
        }
    }
}