using Entities;
using Services;
using EST.MIT.Web.Shared;
using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Pages.approvals.ApprovalConfirm;

public partial class ApprovalConfirm : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IPageServices _pageServices { get; set; }
    [Inject] private IApprovalService _approvalService { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }

    private Invoice invoice;
    private Approval _approval = new();
    private bool IsErrored;
    private Dictionary<string, List<string>> Errors = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        invoice = _invoiceStateContainer.Value;
    }

    private async Task ApproveConfirmed()
    {
        await _approvalService.ApproveInvoiceAsync(invoice).ContinueWith(x =>
        {
            if (x.Result)
            {
                _nav.NavigateTo($"/approval/confirmation/{invoice.Id.ToString()}");
            }
            else
            {
                IsErrored = true;
                Errors.Add("Error", new List<string> { x.Exception?.Message ?? "Unknown Error" });
            }
        });
    }

    private async Task RejectConfirmed()
    {
        await _approvalService.RejectInvoiceAsync(invoice, _approval.Justification).ContinueWith(x =>
        {
            if (x.Result)
            {
                _nav.NavigateTo($"/approval/confirmation/{invoice.Id.ToString()}");
            }
            else
            {
                IsErrored = true;
                Errors.Add("Error", new List<string> { x.Exception?.Message ?? "Unknown Error" });
            }
        });
    }

    private void RejectValidationFailed()
    {
        _pageServices.Validation(_approval, out IsErrored, out Errors);
    }

    private bool IsRejectEndpoint
    {
        get
        {
            var uri = _nav.ToAbsoluteUri(_nav.Uri);
            return uri.AbsolutePath.EndsWith("reject", StringComparison.OrdinalIgnoreCase);
        }
    }

}