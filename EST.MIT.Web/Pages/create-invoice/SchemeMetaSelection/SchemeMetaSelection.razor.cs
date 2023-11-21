using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Helpers;

namespace EST.MIT.Web.Pages.create_invoice.SchemeMetaSelection;

public partial class SchemeMetaSelection : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] public IPageServices _pageServices { get; set; }
    [Inject] private IReferenceDataAPI _referenceDataAPI { get; set; }

    private Invoice invoice = default!;
    private SchemeSelect schemeSelect = new();
    private Dictionary<string, string> schemes = new();
    bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();
    private List<string> viewErrors = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        invoice = _invoiceStateContainer.Value;

        if (invoice != null)
        {
            await _referenceDataAPI.GetSchemeTypesAsync(invoice.AccountType, invoice.Organisation).ContinueWith(x =>
            {
                if (x.Result.IsSuccess)
                {
                    foreach (var scheme in x.Result.Data)
                    {
                        schemes.Add(scheme.code, scheme.description);
                    }
                }
            });
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (_invoiceStateContainer.Value == null || _invoiceStateContainer.Value.IsNull())
        {
            _invoiceStateContainer.SetValue(null);
            _nav.NavigateTo("/create-invoice");
        }
    }

    private void SaveAndContinue()
    {
        invoice.SchemeType = schemeSelect.Scheme;
        _invoiceStateContainer.SetValue(invoice);
        _nav.NavigateTo("/create-invoice/payment-type");
    }

    private void ValidationFailed()
    {
        _pageServices.Validation(schemeSelect, out IsErrored, out errors);
        viewErrors = errors[nameof(schemeSelect.Scheme).ToLower()];
    }

    private void Cancel()
    {
        _invoiceStateContainer.SetValue(null);
        _nav.NavigateTo("/");
    }
}