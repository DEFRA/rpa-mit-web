using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
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
        try
        {
            var result = await _approvalService.ApproveInvoiceAsync(invoice);
            if (result)
            {
                _nav.NavigateTo($"/approval/confirmation/approved/{invoice.Id}");
            }
            else
            {
                IsErrored = true;
                Errors.Add("Error", new List<string> { "Approval failed" });
            }
        }
        catch (Exception ex)
        {
            UpdateError(ex.Message);
        }
    }

    private async Task RejectConfirmed()
    {
        try
        {
            var result = await _approvalService.RejectInvoiceAsync(invoice, _approval.Reason);
            if (result)
            {
                _nav.NavigateTo($"/approval/confirmation/rejected/{invoice.Id}");
            }
            else
            {
                UpdateError("Rejection failed");
            }
        }
        catch (Exception ex)
        {
            UpdateError(ex.Message);
        }
    }

    private void UpdateError(string message)
    {
        IsErrored = true;
        if (!Errors.ContainsKey("Error"))
        {
            Errors.Add("Error", new List<string>());
        }
        Errors["Error"].Add(message);
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