using EST.MIT.Web.Entities;
using EST.MIT.Web.Shared;
using EST.MIT.Web.Helpers;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Services;

namespace EST.MIT.Web.Pages.create_invoice.PaymentTypeMetaSelection;

public partial class PaymentTypeMetaSelection : ComponentBase
{
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] public IPageServices _pageServices { get; set; }
    [Inject] private IReferenceDataAPI _referenceDataAPI { get; set; }

    private Invoice invoice = default!;
    private PaymentTypeSelect paymentTypeSelect = new();
    private readonly Dictionary<string, string> paymentTypes = new();
    bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();
    private List<string> viewErrors = new();
    public bool IsDataLoaded { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        invoice = _invoiceStateContainer.Value;

        if (invoice != null)
        {
            var result = await _referenceDataAPI.GetPaymentTypesAsync(invoice.AccountType, invoice.Organisation, invoice.SchemeType);
            if (result.IsSuccess)
            {
                foreach (var paymentType in result.Data)
                {
                    paymentTypes.Add(paymentType.code, paymentType.description);
                }

                await InvokeAsync(OnDataLoaded);
            }
        }
    }

    private void OnDataLoaded()
    {
        IsDataLoaded = true;
        StateHasChanged();
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
        invoice.PaymentType = paymentTypeSelect.PaymentType;
        _invoiceStateContainer.SetValue(invoice);
        _nav.NavigateTo("/create-invoice/review");
    }

    private void ValidationFailed()
    {
        _pageServices.Validation(paymentTypeSelect, out IsErrored, out errors);
        viewErrors = errors[nameof(paymentTypeSelect.PaymentType).ToLower()];
    }

    private void Cancel()
    {
        _invoiceStateContainer.SetValue(null);
        _nav.NavigateTo("/");
    }
}