using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Pages.approvals.ApprovalRejectionConfirmation;

public partial class ApprovalRejectionConfirmation : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IPageServices _pageServices { get; set; }
    [Inject] private IApprovalService _approvalService { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Parameter] public string Id { get; set; }

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
                _nav.NavigateTo($"/approval/confirmation/{invoice.Id.ToString()}");
            }
            else
            {
                UpdateError("Unknown Error");
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
            var result = await _approvalService.RejectInvoiceAsync(invoice, _approval.Justification);

            if (result)
            {
                _nav.NavigateTo($"/approval/confirmation/{invoice.Id.ToString()}");
            }
            else
            {
                UpdateError("Unknown Error");
            }
        }
        catch (Exception ex)
        {
            UpdateError(ex.Message);
        }
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
            return uri.AbsolutePath.EndsWith("rejected", StringComparison.OrdinalIgnoreCase);
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
}