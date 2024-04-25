using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Helpers;

namespace EST.MIT.Web.Pages.invoice.AddPaymentRequest;

public partial class AddPaymentRequest : ComponentBase
{
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IPageServices _pageServices { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }

    private readonly Dictionary<string, string> currencies = new();


    [Parameter] public Invoice invoice { get; set; } = default!;

    private readonly PaymentRequest paymentRequest = new();
    private bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();

    private List<string> CustomerReferenceCommonKeys { get; } = new List<string> { "CustomerReference" };

    public AddPaymentRequest()
    {
        this.currencies.Add("GBP", "GBP");
        this.currencies.Add("EUR", "EUR");
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        invoice ??= _invoiceStateContainer.Value;
        paymentRequest.AccountType = invoice?.AccountType;
        this.paymentRequest.Currency = this.currencies.First().Value;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (invoice.IsNull())
        {
            _invoiceStateContainer.SetValue(null);
            _nav.NavigateTo("/");
        }
    }

    private async Task SaveHeader()
    {
        if (!_pageServices.Validation(paymentRequest, out IsErrored, out errors)) return;

        var response = await _api.UpdateInvoiceAsync(invoice, paymentRequest);

        if (response.IsSuccess)
        {
            IsErrored = false;
            errors.Clear();
            _invoiceStateContainer.SetValue(response.Data);
            _nav.NavigateTo($"/invoice/summary/{invoice.SchemeType}/{invoice.Id}");
        }
        else
        {
            IsErrored = true;
            errors = paymentRequest.Errors;
        }
    }

    private void Cancel()
    {
        _nav.NavigateTo($"/invoice/summary/{invoice.SchemeType}/{invoice.Id}");
    }
}