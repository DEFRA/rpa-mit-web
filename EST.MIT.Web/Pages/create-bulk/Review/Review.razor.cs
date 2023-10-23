using Entities;
using Services;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Shared;
using Helpers;

namespace EST.MIT.Web.Pages.create_bulk.Review;

public partial class Review : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private IPageServices _pageServices { get; set; }
    [Inject] private IInvoiceAPI _api { get; set; }

    private Invoice invoice { get; set; }
    private bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        invoice = _invoiceStateContainer.Value;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (_invoiceStateContainer.Value == null || _invoiceStateContainer.Value.IsNull())
        {
            _invoiceStateContainer.SetValue(null);
            _nav.NavigateTo("/create-bulk");
        }
    }

    private void ValidationFailed()
    {
        _pageServices.Validation(invoice, out IsErrored, out errors);
    }

    private void SaveAndContinue()
    {
        var createdBy = "user";
        _nav.NavigateTo($"/bulk/{invoice.SchemeType}/{invoice.Organisation}/{invoice.PaymentType}/{invoice.AccountType}/{createdBy}");
    }

    private void Cancel()
    {
        _invoiceStateContainer.SetValue(null);
        _nav.NavigateTo("/");
    }
}