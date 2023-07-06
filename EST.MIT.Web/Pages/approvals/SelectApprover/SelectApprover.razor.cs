using Entities;
using EST.MIT.Web.Shared;
using Microsoft.AspNetCore.Components;
using Services;

namespace EST.MIT.Web.Pages.approvals.SelectApprover;

public partial class SelectApprover : ComponentBase
{
    [Inject] public IInvoiceStateContainer _invoiceStateContainer { get; set; } = default!;
    [Inject] public IPageServices _pageServices { get; set; } = default!;
    [Inject] public IApprovalService _approvalService { get; set; } = default!;
    [Inject] public NavigationManager _nav { get; set; } = default!;

    private Invoice invoice = default!;
    private Approver approver = new();
    Dictionary<string, string> approvers = new Dictionary<string, string>
    {
        {"Peter@defra.gov.uk", "Peter"},
        {"Bjorn@apha.gov.uk", "Bjorn"},
        {"John@rpa.gov.uk", "John"}
    };
    bool IsErrored = false;
    private Dictionary<string, string> errors = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        invoice = _invoiceStateContainer.Value;
    }

    private void ValidationFailed()
    {
        _pageServices.Validation(approver, out IsErrored, out errors);
    }

    private async Task SubmitApproval()
    {
        invoice.Approver = approver.Name;

        var response = await _approvalService.SubmitApprovalAsync(invoice);
        if (response)
        {
            _nav.NavigateTo($"/approval/confirmation/{invoice.Id}");
        }
    }
}