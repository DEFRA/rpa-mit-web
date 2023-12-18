using EST.MIT.Web.Entities;
using EST.MIT.Web.Helpers;
using Microsoft.AspNetCore.Components;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Pages.invoice.AmendInvoiceLine;

public partial class AmendInvoiceLine : ComponentBase
{
    [Inject] private IInvoiceAPI _api { get; set; }
    [Inject] private NavigationManager _nav { get; set; }
    [Inject] private IPageServices _pageServices { get; set; }
    [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }
    [Inject] private IReferenceDataAPI _referenceDataAPI { get; set; }

    [Parameter] public string PaymentRequestId { get; set; } = "";
    [Parameter] public string InvoiceLineId { get; set; } = "";

    private Invoice invoice;
    private PaymentRequest paymentRequest;
    private InvoiceLine invoiceLine;
    private bool IsErrored = false;
    private Dictionary<string, List<string>> errors = new();
    private readonly Dictionary<string, string> allFundCodes = new();
    private readonly Dictionary<string, string> allAccounts = new();
    private readonly Dictionary<string, string> allSchemeCodes = new();
    private readonly Dictionary<string, string> allMarketingYears = new();
    private readonly Dictionary<string, string> allDeliveryBodies = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        invoice ??= _invoiceStateContainer.Value;
        paymentRequest ??= invoice?.PaymentRequests.First(x => x.PaymentRequestId == PaymentRequestId);
        invoiceLine ??= paymentRequest?.InvoiceLines.First(x => x.Id.ToString() == InvoiceLineId);

        var accountsResult = await _referenceDataAPI.GetAccountsAsync(invoice?.AccountType, invoice?.Organisation, invoice?.SchemeType, invoice?.PaymentType);
        if (accountsResult.IsSuccess)
        {
            foreach (var account in accountsResult.Data)
            {
                allAccounts.Add(account.code, account.description);
            }
        }

        var deliveryBodiesResult = await _referenceDataAPI.GetDeliveryBodiesAsync(invoice?.AccountType, invoice?.Organisation, invoice?.SchemeType, invoice?.PaymentType);
        if (deliveryBodiesResult.IsSuccess)
        {
            foreach (var deliveryBody in deliveryBodiesResult.Data)
            {
                allDeliveryBodies.Add(deliveryBody.code, deliveryBody.description);
            }
        }

        var schemesResult = await _referenceDataAPI.GetSchemesAsync(invoice?.AccountType, invoice?.Organisation, invoice?.SchemeType, invoice?.PaymentType);
        if (schemesResult.IsSuccess)
        {
            foreach (var scheme in schemesResult.Data)
            {
                allSchemeCodes.Add(scheme.code, scheme.description);
            }
        }

        var marketingYearsResult = await _referenceDataAPI.GetMarketingYearsAsync(invoice?.AccountType, invoice?.Organisation, invoice?.SchemeType, invoice?.PaymentType);
        if (marketingYearsResult.IsSuccess)
        {
            foreach (var marketingYear in marketingYearsResult.Data)
            {
                allMarketingYears.Add(marketingYear.code, marketingYear.description);
            }
        }

        var fundsResult = await _referenceDataAPI.GetFundsAsync(invoice?.AccountType, invoice?.Organisation, invoice?.SchemeType, invoice?.PaymentType);
        if (fundsResult.IsSuccess)
        {
            foreach (var fund in fundsResult.Data)
            {
                allFundCodes.Add(fund.code, fund.description);
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (paymentRequest.IsNull() || invoice.IsNull())
        {
            _invoiceStateContainer.SetValue(null);
            _nav.NavigateTo("/");
        }
    }

    private string ErrorMessagesForField(string fieldKey)
    {
        return ErrorMessageHelper.ErrorMessagesForField(errors, fieldKey);
    }

    private async Task UpdateInvoiceLine()
    {
        if (!_pageServices.Validation(invoiceLine, out IsErrored, out errors)) return;

        paymentRequest.InvoiceLines = paymentRequest.InvoiceLines.Where(x => x.Id.ToString() != InvoiceLineId).ToList();
        paymentRequest.InvoiceLines.Add(invoiceLine);

        var response = await _api.UpdateInvoiceAsync(invoice);

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
            errors = invoiceLine.Errors;
        }
    }

    private void Cancel()
    {
        _nav.NavigateTo($"/invoice/amend-payment-request/{PaymentRequestId}");
    }
}