using EST.MIT.Web.Entities;
using EST.MIT.Web.Helpers;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.invoice.AddInvoiceLine;

public partial class AddInvoiceLine : ComponentBase
{
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IPageServices _pageServices { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private IReferenceDataAPI _referenceDataAPI { get; set; }
    [Inject] private ILogger<AddInvoiceLine> Logger { get; set; }


    [Parameter] public string PaymentRequestId { get; set; } = "";

    private Invoice invoice;
    private PaymentRequest paymentRequest;
    private readonly InvoiceLine invoiceLine = new();
    private bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();
    private readonly Dictionary<string, string> allFundCodes = new();
    private readonly Dictionary<string, string> allAccounts = new();
    private readonly Dictionary<string, string> allSchemeCodes = new();
    private readonly Dictionary<string, string> allMarketingYears = new();
    private readonly Dictionary<string, string> allDeliveryBodies = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();
            invoice ??= _invoiceStateContainer.Value;
            paymentRequest ??= invoice?.PaymentRequests.First(x => x.PaymentRequestId == PaymentRequestId);

            if (invoice != null)
            {
                await PopulateReferenceDataAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in OnInitializedAsync of AddInvoiceLine page");
            _nav.NavigateTo("/error");
        }
    }

    private async Task PopulateReferenceDataAsync()
    {
        await PopulateAccountsAsync();
        await PopulateDeliveryBodiesAsync();
        await PopulateSchemesAsync();
        await PopulateMarketingYearsAsync();
        await PopulateFundsAsync();
    }

    private async Task PopulateAccountsAsync()
    {
        var result = await _referenceDataAPI.GetAccountsAsync(invoice.AccountType, invoice.Organisation, invoice.SchemeType, invoice.PaymentType);
        if (result.IsSuccess)
        {
            foreach (var account in result.Data)
            {
                allAccounts.Add(account.code, account.description);
            }
        }
    }

    private async Task PopulateMarketingYearsAsync()
    {
        var result = await _referenceDataAPI.GetMarketingYearsAsync(invoice.AccountType, invoice.Organisation, invoice.SchemeType, invoice.PaymentType);
        if (result.IsSuccess)
        {
            foreach (var marketingYear in result.Data)
            {
                allMarketingYears.Add(marketingYear.code, marketingYear.description);
            }
        }
    }

    private async Task PopulateFundsAsync()
    {
        var result = await _referenceDataAPI.GetFundsAsync(invoice.AccountType, invoice.Organisation, invoice.SchemeType, invoice.PaymentType);
        if (result.IsSuccess)
        {
            foreach (var fund in result.Data)
            {
                allFundCodes.Add(fund.code, fund.description);
            }
        }
    }

    private async Task PopulateSchemesAsync()
    {
        var result = await _referenceDataAPI.GetSchemesAsync(invoice.AccountType, invoice.Organisation, invoice.SchemeType, invoice.PaymentType);
        if (result.IsSuccess)
        {
            foreach (var scheme in result.Data)
            {
                allSchemeCodes.Add(scheme.code, scheme.description);
            }
        }
    }

    private async Task PopulateDeliveryBodiesAsync()
    {
        var result = await _referenceDataAPI.GetDeliveryBodiesAsync(invoice.AccountType, invoice.Organisation, invoice.SchemeType, invoice.PaymentType);
        if (result.IsSuccess)
        {
            foreach (var deliveryBody in result.Data)
            {
                allDeliveryBodies.Add(deliveryBody.code, deliveryBody.description);
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            await base.OnAfterRenderAsync(firstRender);
            if (paymentRequest.IsNull() || invoice.IsNull())
            {
                _invoiceStateContainer.SetValue(null);
                _nav.NavigateTo("/");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in OnAfterRenderAsync of AddInvoiceLine page");
            _nav.NavigateTo("/error");
        }
    }

    private string ErrorMessagesForField(string fieldKey)
    {
        return ErrorMessageHelper.ErrorMessagesForField(errors, fieldKey);
    }

    private async Task SaveInvoiceLine()
    {
        try
        {
            if (!_pageServices.Validation(invoiceLine, out IsErrored, out errors)) return;

            var response = await _api.UpdateInvoiceAsync(invoice, paymentRequest, invoiceLine);

            if (response.IsSuccess)
            {
                IsErrored = false;
                errors.Clear();
                _invoiceStateContainer.SetValue(response.Data);
                _nav.NavigateTo($"/invoice/amend-payment-request/{PaymentRequestId}");
            }
            else
            {
                IsErrored = true;
                errors = invoice.AllErrors;
            }
        }
        catch (Exception ex)
        {
            IsErrored = true;
            Logger.LogError(ex, "Error in SaveAndContinue of AddInvoiceLine page");
        }
    }

    private async Task Cancel()
    {
        var invoiceBeforeEdit = await _api.FindInvoiceAsync(invoice.Id.ToString(), invoice.SchemeType);
        _invoiceStateContainer.SetValue(invoiceBeforeEdit);
        _nav.NavigateTo($"/invoice/amend-payment-request/{PaymentRequestId}");
    }
}