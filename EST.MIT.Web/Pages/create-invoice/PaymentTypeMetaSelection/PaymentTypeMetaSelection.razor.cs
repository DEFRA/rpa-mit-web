using Entities;
using EST.MIT.Web.Shared;
using Helpers;
using Microsoft.AspNetCore.Components;
using Services;

namespace EST.MIT.Web.Pages.create_invoice.PaymentTypeMetaSelection;

public partial class PaymentTypeMetaSelection : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] public IPageServices _pageServices { get; set; }

    private Invoice invoice = default!;
    private PaymentTypeSelect paymentTypeSelect = new();
    private readonly Dictionary<string, string> paymentTypes = new()
    {
         { "EU", "EU Funds"},
         { "DOM", "Domestic Funds" }
    };
    bool IsErrored = false;
    private Dictionary<string, string> errors = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        invoice = _invoiceStateContainer.Value;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (_invoiceStateContainer.Value.IsNull())
        {
            _invoiceStateContainer.SetValue(null);
            _nav.NavigateTo("/create-invoice");
        }
    }
    private void SaveAndContinue()
    {
        invoice.PaymentType = paymentTypeSelect.PaymentType;
        _invoiceStateContainer.SetValue(invoice);
        _nav.NavigateTo("/create-invoice/review");
    }

    private void ValidationFailed()
    {
        _pageServices.Validation(paymentTypeSelect, out IsErrored, out errors);
    }

    private void Cancel()
    {
        _invoiceStateContainer.SetValue(null);
        _nav.NavigateTo("/");
    }
}