using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Shared.Components.ApprovalCard;

public partial class ApprovalCard : ComponentBase
{
    [Parameter] public Invoice invoice { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private NavigationManager _nav { get; set; }

    private string backLink = "/approval/mine";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
    }

    private void Approve()
    {
        _invoiceStateContainer.SetValue(invoice);
        _nav.NavigateTo("/approval/confirm/approve");
    }

    private void Reject()
    {
        _invoiceStateContainer.SetValue(invoice);
        _nav.NavigateTo("/approval/confirm/reject");
    }

}