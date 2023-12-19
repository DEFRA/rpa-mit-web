using EST.MIT.Web.Entities;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.approvals.SelectApprover;

public partial class SelectApprover : ComponentBase
{
    [Inject] public IInvoiceStateContainer _invoiceStateContainer { get; set; } = default!;
    [Inject] public IPageServices _pageServices { get; set; } = default!;
    [Inject] public IApprovalService _approvalService { get; set; } = default!;
    [Inject] public NavigationManager _nav { get; set; } = default!;
    [Inject] public ILogger<SelectApprover> Logger { get; set; } = default!;

    private Invoice invoice = default!;
    private readonly ApproverSelect approverSelect = new();
    private bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();
    private bool ShowErrorSummary = false;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();
            invoice ??= _invoiceStateContainer.Value;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing SelectApprover page");
            _nav.NavigateTo("/error");
        }
    }

    private void ValidationFailed()
    {
        ShowErrorSummary = false;
        _pageServices.Validation(approverSelect, out IsErrored, out errors);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (_invoiceStateContainer.Value == null)
        {
            _invoiceStateContainer.SetValue(null);
            _nav.NavigateTo("/");
        }
    }

    private void Cancel()
    {
        _nav.NavigateTo($"/invoice/summary/{invoice.SchemeType}/{invoice.Id}");
    }

    private async Task SubmitApproval()
    {
        try
        {
            IsErrored = false;
            var validate = await _approvalService.ValidateApproverAsync(approverSelect.ApproverEmail, invoice.ApprovalGroup);

            if (!validate.IsSuccess || !validate.Data.Value)
            {
                ShowErrorSummary = IsErrored == false;
                errors = validate.Errors;
                return;
            }

            invoice.ApproverId = "user"; //TODO: fxs this needs to be the approver id, for now hard coded to 1
            invoice.ApproverEmail = approverSelect.ApproverEmail;

            var response = await _approvalService.SubmitApprovalAsync(invoice);
            if (!response.IsSuccess)
            {
                if (invoice.AllErrors.Count > 0)
                {
                    ShowErrorSummary = true;
                    errors = invoice.AllErrors;
                    return;
                }
            }

            _nav.NavigateTo($"/approval/confirmation/{invoice.Id}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during SubmitApproval");
            _nav.NavigateTo("/error");
        }
    }
}