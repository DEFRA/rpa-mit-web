using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Helpers;

namespace EST.MIT.Web.Pages.create_invoice.Review;

public partial class Review : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private IPageServices _pageServices { get; set; }
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private ILogger<Review> Logger { get; set; }

    private Invoice invoice = default!;
    private bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();
            invoice = _invoiceStateContainer.Value;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing Review page");
            _nav.NavigateTo("/error");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            await base.OnAfterRenderAsync(firstRender);
            if (_invoiceStateContainer.Value == null || _invoiceStateContainer.Value.IsNull())
            {
                _invoiceStateContainer.SetValue(null);
                _nav.NavigateTo("/create-invoice");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in OnAfterRenderAsync of Review page");
            _nav.NavigateTo("/error");
        }
    }


    private void ValidationFailed()
    {
        _pageServices.Validation(invoice, out IsErrored, out errors);
    }

    private async Task SaveAndContinue()
    {
        var response = await _api.SaveInvoiceAsync(invoice);

        if (response.IsSuccess)
        {
            _invoiceStateContainer.SetValue(invoice);
            _nav.NavigateTo($"/invoice/summary/{invoice.SchemeType}/{invoice.Id}");
        }

        IsErrored = true;
        errors = response.Errors;
    }

    private void Cancel()
    {
        _invoiceStateContainer.SetValue(null);
        _nav.NavigateTo("/");
    }
}