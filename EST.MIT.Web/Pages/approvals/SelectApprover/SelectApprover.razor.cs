using EST.MIT.Web.Entities;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using EST.MIT.Web.Helpers;
using System.Net;
using EST.MIT.Web.Shared;

namespace EST.MIT.Web.Pages.approvals.SelectApprover;

public partial class SelectApprover : ComponentBase
{
    [Inject] public IInvoiceStateContainer _invoiceStateContainer { get; set; } = default!;
    [Inject] public IPageServices _pageServices { get; set; } = default!;
    [Inject] public IApprovalService _approvalService { get; set; } = default!;
    [Inject] public NavigationManager _nav { get; set; } = default!;
    [Inject] public ILogger<SelectApprover> Logger { get; set; } = default!;

    [CascadingParameter]
    public MainLayout Layout { get; set; }

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
            // var validate = await _approvalService.ValidateApproverAsync(approverSelect.ApproverEmail, invoice.ApprovalGroup);
            var validate = new ApiResponse<BoolRef>(true) { Data = new BoolRef(true) }; // TODO: Remove this line when the above line is uncommented

            if (!validate.IsSuccess || !validate.Data.Value)
            {
                ShowErrorSummary = IsErrored == false;
                errors = validate.Errors;
                return;
            }

            invoice.ApproverId = approverSelect.ApproverEmail; // TODO Can/Should this be a userId rather than the email for the user?
            invoice.ApproverEmail = approverSelect.ApproverEmail;
            invoice.ApprovalRequestedByEmail = Layout.UserEmail;

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